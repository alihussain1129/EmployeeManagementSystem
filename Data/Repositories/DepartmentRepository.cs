using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DatabaseContext _context;

        public DepartmentRepository(DatabaseContext context)
        {
            _context = context;
        }

        public List<Department> GetAll()
        {
            var departments = new List<Department>();

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "SELECT DepartmentId, Name FROM Departments ORDER BY Name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                }
            }

            return departments;
        }

        public Department GetById(int departmentId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "SELECT DepartmentId, Name FROM Departments WHERE DepartmentId = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Department
                            {
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                    }
                }
            }

            return null;
        }

        public int Add(Department department)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Departments (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", department.Name);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Department department)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Departments SET Name = @Name WHERE DepartmentId = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", department.Name);
                    cmd.Parameters.AddWithValue("@DepartmentId", department.DepartmentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int departmentId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Departments WHERE DepartmentId = @DepartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}