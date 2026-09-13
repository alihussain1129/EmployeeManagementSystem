using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
    }
}