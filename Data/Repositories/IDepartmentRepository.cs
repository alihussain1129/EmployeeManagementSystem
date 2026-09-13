using System.Collections.Generic;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public interface IDepartmentRepository
    {
        List<Department> GetAll();
        Department GetById(int departmentId);
        int Add(Department department);
        bool Update(Department department);
        bool Delete(int departmentId);
    }
}