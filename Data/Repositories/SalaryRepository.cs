using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly DatabaseContext _context;

        public SalaryRepository(DatabaseContext context)
        {
            _context = context;
        }

        private const string BaseSelect = @"
            SELECT s.SalaryId, s.EmployeeId, e.FullName AS EmployeeName,
                   s.BasicSalary, s.Allowances, s.Deductions, s.EffectiveMonth
            FROM Salaries s
            LEFT JOIN Employees e ON s.EmployeeId = e.EmployeeId";

        public List<Salary> GetAll()
        {
            var salaries = new List<Salary>();

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " ORDER BY s.EffectiveMonth DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        salaries.Add(MapReader(reader));
                }
            }

            return salaries;
        }

        public List<Salary> GetByEmployeeId(int employeeId)
        {
            var salaries = new List<Salary>();

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " WHERE s.EmployeeId = @EmployeeId ORDER BY s.EffectiveMonth DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            salaries.Add(MapReader(reader));
                    }
                }
            }

            return salaries;
        }

        public Salary GetById(int salaryId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " WHERE s.SalaryId = @SalaryId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SalaryId", salaryId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapReader(reader);
                    }
                }
            }

            return null;
        }

        public int Add(Salary salary)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO Salaries (EmployeeId, BasicSalary, Allowances, Deductions, EffectiveMonth)
                    VALUES (@EmployeeId, @BasicSalary, @Allowances, @Deductions, @EffectiveMonth);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, salary);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Salary salary)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = @"
                    UPDATE Salaries SET
                        EmployeeId = @EmployeeId, BasicSalary = @BasicSalary,
                        Allowances = @Allowances, Deductions = @Deductions, EffectiveMonth = @EffectiveMonth
                    WHERE SalaryId = @SalaryId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    AddParams(cmd, salary);
                    cmd.Parameters.AddWithValue("@SalaryId", salary.SalaryId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int salaryId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Salaries WHERE SalaryId = @SalaryId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SalaryId", salaryId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private static void AddParams(SqlCommand cmd, Salary s)
        {
            cmd.Parameters.AddWithValue("@EmployeeId", s.EmployeeId);
            cmd.Parameters.AddWithValue("@BasicSalary", s.BasicSalary);
            cmd.Parameters.AddWithValue("@Allowances", s.Allowances);
            cmd.Parameters.AddWithValue("@Deductions", s.Deductions);
            cmd.Parameters.AddWithValue("@EffectiveMonth", s.EffectiveMonth);
        }

        private static Salary MapReader(SqlDataReader reader)
        {
            return new Salary
            {
                SalaryId = reader.GetInt32(reader.GetOrdinal("SalaryId")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? "" : reader.GetString(reader.GetOrdinal("EmployeeName")),
                BasicSalary = reader.GetDecimal(reader.GetOrdinal("BasicSalary")),
                Allowances = reader.GetDecimal(reader.GetOrdinal("Allowances")),
                Deductions = reader.GetDecimal(reader.GetOrdinal("Deductions")),
                EffectiveMonth = reader.GetDateTime(reader.GetOrdinal("EffectiveMonth"))
            };
        }
    }
}