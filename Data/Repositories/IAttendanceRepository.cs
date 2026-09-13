using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public interface IAttendanceRepository
    {
        List<Attendance> GetAll();
        List<Attendance> GetByEmployeeAndDateRange(int employeeId, DateTime from, DateTime to);
        int Add(Attendance attendance);
        bool Delete(int attendanceId);
    }
}