using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public interface ISalaryRepository
    {
        List<Salary> GetAll();
        List<Salary> GetByEmployeeId(int employeeId);
        Salary GetById(int salaryId);
        int Add(Salary salary);
        bool Update(Salary salary);
        bool Delete(int salaryId);
    }
}