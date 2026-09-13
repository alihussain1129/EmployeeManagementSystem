using System;
using System.Linq;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly EmployeeService _employeeService;
        private readonly DepartmentService _departmentService;
        private readonly AttendanceService _attendanceService;

        private int _totalEmployees;
        public int TotalEmployees { get => _totalEmployees; set { _totalEmployees = value; OnPropertyChanged(); } }

        private int _activeEmployees;
        public int ActiveEmployees { get => _activeEmployees; set { _activeEmployees = value; OnPropertyChanged(); } }

        private int _totalDepartments;
        public int TotalDepartments { get => _totalDepartments; set { _totalDepartments = value; OnPropertyChanged(); } }

        private int _todayAttendanceCount;
        public int TodayAttendanceCount { get => _todayAttendanceCount; set { _todayAttendanceCount = value; OnPropertyChanged(); } }

        public RelayCommand RefreshCommand { get; }

        public DashboardViewModel(EmployeeService employeeService, DepartmentService departmentService, AttendanceService attendanceService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _attendanceService = attendanceService;

            RefreshCommand = new RelayCommand(_ => LoadStats());

            LoadStats();
        }

        public void LoadStats()
        {
            var employees = _employeeService.GetAllEmployees();
            var departments = _departmentService.GetAllDepartments();
            var attendance = _attendanceService.GetAllAttendance();

            TotalEmployees = employees.Count;
            ActiveEmployees = employees.Count(e => e.Status == "Active");
            TotalDepartments = departments.Count;

            TodayAttendanceCount = attendance
                .Where(a => a.PunchTime.Date == DateTime.Today && a.PunchType == "IN")
                .Select(a => a.EmployeeId)
                .Distinct()
                .Count();
        }
    }
}