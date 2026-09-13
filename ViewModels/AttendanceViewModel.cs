using System;
using System.Collections.ObjectModel;
using System.Windows;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.ViewModels
{
    public class AttendanceViewModel : BaseViewModel
    {
        private readonly AttendanceService _attendanceService;
        private readonly EmployeeService _employeeService;
        private readonly string _currentUserRole;

        public bool IsAdmin => _currentUserRole == "Admin";

        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<DailyAttendanceSummary> DailySummaries { get; set; }
        public ObservableCollection<Attendance> RawPunches { get; set; }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        private DateTime _fromDate = DateTime.Today.AddDays(-7);
        public DateTime FromDate { get => _fromDate; set { _fromDate = value; OnPropertyChanged(); } }

        private DateTime _toDate = DateTime.Today;
        public DateTime ToDate { get => _toDate; set { _toDate = value; OnPropertyChanged(); } }

        private Attendance _selectedPunch;
        public Attendance SelectedPunch
        {
            get => _selectedPunch;
            set { _selectedPunch = value; OnPropertyChanged(); }
        }

        public RelayCommand SearchCommand { get; }
        public RelayCommand PunchInCommand { get; }
        public RelayCommand PunchOutCommand { get; }
        public RelayCommand DeletePunchCommand { get; }

        public AttendanceViewModel(AttendanceService attendanceService, EmployeeService employeeService, string currentUserRole)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
            _currentUserRole = currentUserRole;

            Employees = new ObservableCollection<Employee>();
            DailySummaries = new ObservableCollection<DailyAttendanceSummary>();
            RawPunches = new ObservableCollection<Attendance>();

            SearchCommand = new RelayCommand(_ => LoadSummary(), _ => SelectedEmployee != null);
            PunchInCommand = new RelayCommand(_ => RecordManualPunch("IN"), _ => SelectedEmployee != null);
            PunchOutCommand = new RelayCommand(_ => RecordManualPunch("OUT"), _ => SelectedEmployee != null);
            DeletePunchCommand = new RelayCommand(_ => DeletePunch(), _ => SelectedPunch != null && IsAdmin);

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var e in _employeeService.GetAllEmployees())
                Employees.Add(e);
        }

        private void LoadSummary()
        {
            if (SelectedEmployee == null) return;

            DailySummaries.Clear();
            var summaries = _attendanceService.GetDailySummary(SelectedEmployee.EmployeeId, FromDate, ToDate);
            foreach (var s in summaries)
                DailySummaries.Add(s);

            RawPunches.Clear();
            var punches = _attendanceService.GetAttendanceForEmployee(SelectedEmployee.EmployeeId, FromDate, ToDate);
            foreach (var p in punches)
                RawPunches.Add(p);
        }

        private void RecordManualPunch(string punchType)
        {
            if (SelectedEmployee == null) return;

            try
            {
                _attendanceService.ManualPunch(SelectedEmployee.EmployeeId, DateTime.Now, punchType);
                LoadSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePunch()
        {
            if (SelectedPunch == null) return;

            if (!IsAdmin)
            {
                MessageBox.Show("Only Admin users can delete attendance records.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Delete this {SelectedPunch.PunchType} punch at {SelectedPunch.PunchTime:dd-MMM-yyyy hh:mm tt}?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _attendanceService.DeleteAttendance(SelectedPunch.AttendanceId);
                LoadSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}