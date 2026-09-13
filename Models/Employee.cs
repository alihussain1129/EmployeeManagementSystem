using System;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string CNIC { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }   // populated via JOIN for grid display
        public DateTime JoiningDate { get; set; }
        public string BiometricId { get; set; }
        public string Status { get; set; }
    }
}