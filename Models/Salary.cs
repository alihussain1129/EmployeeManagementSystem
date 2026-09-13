using System;

namespace EmployeeManagementSystem.Models
{
    public class Salary
    {
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }   // populated via JOIN for grid display
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public DateTime EffectiveMonth { get; set; }

        public decimal NetSalary => BasicSalary + Allowances - Deductions;
    }
}