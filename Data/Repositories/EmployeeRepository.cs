using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DatabaseContext _context;

        public EmployeeRepository(DatabaseContext context)
        {
            _context = context;
        }

        private const string BaseSelect = @"
            SELECT e.EmployeeId, e.FullName, e.CNIC, e.Designation, e.DepartmentId,
                   d.Name AS DepartmentName, e.JoiningDate, e.BiometricId, e.Status
            FROM Employees e
            LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId";

        public List<Employee> GetAll()
        {
            var employees = new List<Employee>();

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " ORDER BY e.FullName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        employees.Add(MapReader(reader));
                }
            }

            return employees;
        }

        public Employee GetById(int employeeId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " WHERE e.EmployeeId = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapReader(reader);
                    }
                }
            }

            return null;
        }

        public Employee GetByBiometricId(string biometricId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " WHERE e.BiometricId = @BiometricId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BiometricId", biometricId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapReader(reader);
                    }
                }
            }

            return null;
        }

        public int Add(Employee employee)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO Employees (FullName, CNIC, Designation, DepartmentId, JoiningDate, BiometricId, Status)
                    VALUES (@FullName, @CNIC, @Designation, @DepartmentId, @JoiningDate, @BiometricId, @Status);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, employee);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Employee employee)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = @"
                    UPDATE Employees SET
                        FullName = @FullName, CNIC = @CNIC, Designation = @Designation,
                        DepartmentId = @DepartmentId, JoiningDate = @JoiningDate,
                        BiometricId = @BiometricId, Status = @Status
                    WHERE EmployeeId = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, employee);
                    cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int employeeId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Employees WHERE EmployeeId = @EmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private static void AddParams(SqlCommand cmd, Employee e)
        {
            cmd.Parameters.AddWithValue("@FullName", e.FullName);
            cmd.Parameters.AddWithValue("@CNIC", e.CNIC);
            cmd.Parameters.AddWithValue("@Designation", e.Designation);
            cmd.Parameters.AddWithValue("@DepartmentId", e.DepartmentId);
            cmd.Parameters.AddWithValue("@JoiningDate", e.JoiningDate);
            cmd.Parameters.AddWithValue("@BiometricId", (object)e.BiometricId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", e.Status);
        }

        private static Employee MapReader(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                CNIC = reader.GetString(reader.GetOrdinal("CNIC")),
                Designation = reader.GetString(reader.GetOrdinal("Designation")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? "" : reader.GetString(reader.GetOrdinal("DepartmentName")),
                JoiningDate = reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
                BiometricId = reader.IsDBNull(reader.GetOrdinal("BiometricId")) ? "" : reader.GetString(reader.GetOrdinal("BiometricId")),
                Status = reader.GetString(reader.GetOrdinal("Status"))
            };
        }
    }
}