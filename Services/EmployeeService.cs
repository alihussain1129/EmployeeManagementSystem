using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmployeeManagementSystem.Data.Repositories;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public List<Employee> GetAllEmployees() => _repository.GetAll();
        public Employee GetEmployeeById(int id) => _repository.GetById(id);

        public int AddEmployee(Employee employee)
        {
            Validate(employee);
            return _repository.Add(employee);
        }

        public bool UpdateEmployee(Employee employee)
        {
            Validate(employee);
            return _repository.Update(employee);
        }

        public bool DeleteEmployee(int id) => _repository.Delete(id);

        private void Validate(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(employee.CNIC))
                throw new ArgumentException("CNIC is required.");
            if (employee.DepartmentId <= 0)
                throw new ArgumentException("Department is required.");
        }

        public void ExportToCsv(string filePath, IEnumerable<Employee> employees)
        {
            var sb = new StringBuilder();
            sb.AppendLine("EmployeeId,FullName,CNIC,Designation,Department,JoiningDate,BiometricId,Status");

            foreach (var e in employees)
            {
                sb.AppendLine(string.Join(",",
                    e.EmployeeId,
                    CsvEscape(e.FullName),
                    CsvEscape(e.CNIC),
                    CsvEscape(e.Designation),
                    CsvEscape(e.DepartmentName),
                    e.JoiningDate.ToString("yyyy-MM-dd"),
                    CsvEscape(e.BiometricId),
                    CsvEscape(e.Status)));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }
    }
}