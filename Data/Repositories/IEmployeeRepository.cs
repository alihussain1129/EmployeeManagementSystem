using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();
        Employee GetById(int employeeId);
        Employee GetByBiometricId(string biometricId);
        int Add(Employee employee);
        bool Update(Employee employee);
        bool Delete(int employeeId);
    }
}