using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Data.Repositories;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class SalaryService
    {
        private readonly ISalaryRepository _repository;

        public SalaryService(ISalaryRepository repository)
        {
            _repository = repository;
        }

        public List<Salary> GetAllSalaries() => _repository.GetAll();
        public List<Salary> GetSalariesForEmployee(int employeeId) => _repository.GetByEmployeeId(employeeId);
        public Salary GetSalaryById(int id) => _repository.GetById(id);

        public int AddSalary(Salary salary)
        {
            Validate(salary);
            return _repository.Add(salary);
        }

        public bool UpdateSalary(Salary salary)
        {
            Validate(salary);
            return _repository.Update(salary);
        }

        public bool DeleteSalary(int id) => _repository.Delete(id);

        private void Validate(Salary salary)
        {
            if (salary.EmployeeId <= 0)
                throw new ArgumentException("Employee is required.");
            if (salary.BasicSalary <= 0)
                throw new ArgumentException("Basic salary must be greater than zero.");
        }
    }
}