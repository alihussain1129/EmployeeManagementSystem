using System;
using System.Security.Cryptography;
using System.Text;
using EmployeeManagementSystem.Data.Repositories;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class AuthService
    {
        private readonly IUserRepository _repository;

        public AuthService(IUserRepository repository)
        {
            _repository = repository;
        }

        public User Login(string username, string password)
        {
            var user = _repository.GetByUsername(username);
            if (user == null) return null;

            string hash = HashPassword(password);
            return string.Equals(hash, user.PasswordHash, StringComparison.OrdinalIgnoreCase) ? user : null;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}