using System;

namespace EmployeeManagementSystem.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }   // populated via JOIN for grid display
        public DateTime PunchTime { get; set; }
        public string PunchType { get; set; }      // "IN" or "OUT"
        public string DeviceId { get; set; }
    }
}