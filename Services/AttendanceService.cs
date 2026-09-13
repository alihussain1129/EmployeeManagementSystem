using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagementSystem.Data.Repositories;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class DailyAttendanceSummary
    {
        public DateTime Date { get; set; }
        public DateTime? FirstIn { get; set; }
        public DateTime? LastOut { get; set; }
        public double HoursWorked { get; set; }
    }

    public class AttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository, IEmployeeRepository employeeRepository)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
        }

        public List<Attendance> GetAllAttendance() => _attendanceRepository.GetAll();

        public List<Attendance> GetAttendanceForEmployee(int employeeId, DateTime from, DateTime to)
            => _attendanceRepository.GetByEmployeeAndDateRange(employeeId, from, to);

        // Called by BiometricListenerService when a raw punch "BiometricId,yyyyMMddHHmmss,IN/OUT" arrives
        public bool RecordPunch(string biometricId, DateTime punchTime, string punchType, string deviceId)
        {
            var employee = _employeeRepository.GetByBiometricId(biometricId);
            if (employee == null) return false;

            _attendanceRepository.Add(new Attendance
            {
                EmployeeId = employee.EmployeeId,
                PunchTime = punchTime,
                PunchType = punchType,
                DeviceId = deviceId
            });

            return true;
        }

        // Manual punch entry from the UI (no biometric device needed)
        public void ManualPunch(int employeeId, DateTime punchTime, string punchType)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee is required.");
            if (punchType != "IN" && punchType != "OUT")
                throw new ArgumentException("Punch type must be IN or OUT.");

            _attendanceRepository.Add(new Attendance
            {
                EmployeeId = employeeId,
                PunchTime = punchTime,
                PunchType = punchType,
                DeviceId = "Manual"
            });
        }

        public bool DeleteAttendance(int attendanceId) => _attendanceRepository.Delete(attendanceId);

        public List<DailyAttendanceSummary> GetDailySummary(int employeeId, DateTime from, DateTime to)
        {
            var punches = GetAttendanceForEmployee(employeeId, from, to);

            return punches
                .GroupBy(p => p.PunchTime.Date)
                .Select(g =>
                {
                    var ins = g.Where(p => p.PunchType == "IN").OrderBy(p => p.PunchTime).ToList();
                    var outs = g.Where(p => p.PunchType == "OUT").OrderBy(p => p.PunchTime).ToList();

                    DateTime? firstIn = ins.FirstOrDefault()?.PunchTime;
                    DateTime? lastOut = outs.LastOrDefault()?.PunchTime;

                    double hours = 0;
                    if (firstIn.HasValue && lastOut.HasValue && lastOut > firstIn)
                        hours = (lastOut.Value - firstIn.Value).TotalHours;

                    return new DailyAttendanceSummary
                    {
                        Date = g.Key,
                        FirstIn = firstIn,
                        LastOut = lastOut,
                        HoursWorked = Math.Round(hours, 2)
                    };
                })
                .OrderBy(s => s.Date)
                .ToList();
        }
    }
}