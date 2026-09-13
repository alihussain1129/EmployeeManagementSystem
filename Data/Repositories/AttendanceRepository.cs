using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Data.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DatabaseContext _context;

        public AttendanceRepository(DatabaseContext context)
        {
            _context = context;
        }

        private const string BaseSelect = @"
            SELECT a.AttendanceId, a.EmployeeId, e.FullName AS EmployeeName,
                   a.PunchTime, a.PunchType, a.DeviceId
            FROM Attendance a
            LEFT JOIN Employees e ON a.EmployeeId = e.EmployeeId";

        public List<Attendance> GetAll()
        {
            var list = new List<Attendance>();

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + " ORDER BY a.PunchTime DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapReader(reader));
                }
            }

            return list;
        }

        public List<Attendance> GetByEmployeeAndDateRange(int employeeId, DateTime from, DateTime to)
        {
            var list = new List<Attendance>();

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = BaseSelect + @"
                    WHERE a.EmployeeId = @EmployeeId
                      AND a.PunchTime >= @FromDate
                      AND a.PunchTime < @ToDate
                    ORDER BY a.PunchTime ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@FromDate", from.Date);
                    cmd.Parameters.AddWithValue("@ToDate", to.Date.AddDays(1));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(MapReader(reader));
                    }
                }
            }

            return list;
        }

        public int Add(Attendance attendance)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO Attendance (EmployeeId, PunchTime, PunchType, DeviceId)
                    VALUES (@EmployeeId, @PunchTime, @PunchType, @DeviceId);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", attendance.EmployeeId);
                    cmd.Parameters.AddWithValue("@PunchTime", attendance.PunchTime);
                    cmd.Parameters.AddWithValue("@PunchType", attendance.PunchType);
                    cmd.Parameters.AddWithValue("@DeviceId", (object)attendance.DeviceId ?? DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Delete(int attendanceId)
        {
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM Attendance WHERE AttendanceId = @AttendanceId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AttendanceId", attendanceId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private static Attendance MapReader(SqlDataReader reader)
        {
            return new Attendance
            {
                AttendanceId = reader.GetInt32(reader.GetOrdinal("AttendanceId")),
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? "" : reader.GetString(reader.GetOrdinal("EmployeeName")),
                PunchTime = reader.GetDateTime(reader.GetOrdinal("PunchTime")),
                PunchType = reader.GetString(reader.GetOrdinal("PunchType")),
                DeviceId = reader.IsDBNull(reader.GetOrdinal("DeviceId")) ? "" : reader.GetString(reader.GetOrdinal("DeviceId"))
            };
        }
    }
}