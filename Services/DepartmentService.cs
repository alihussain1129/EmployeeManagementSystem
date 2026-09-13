using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Data.Repositories;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class DepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public List<Department> GetAllDepartments() => _repository.GetAll();

        public Department GetDepartmentById(int id) => _repository.GetById(id);

        public int AddDepartment(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.Name))
                throw new ArgumentException("Department name is required.");

            return _repository.Add(department);
        }

        public bool UpdateDepartment(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.Name))
                throw new ArgumentException("Department name is required.");

            return _repository.Update(department);
        }

        public bool DeleteDepartment(int id) => _repository.Delete(id);
    }
}