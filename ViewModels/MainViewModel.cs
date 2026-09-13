using System.Windows;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public DashboardViewModel DashboardVM { get; }
        public EmployeeViewModel EmployeeVM { get; }
        public SalaryViewModel SalaryVM { get; }
        public AttendanceViewModel AttendanceVM { get; }
        public DepartmentViewModel DepartmentVM { get; }

        public User CurrentUser { get; }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand ShowEmployeeCommand { get; }
        public RelayCommand ShowSalaryCommand { get; }
        public RelayCommand ShowAttendanceCommand { get; }
        public RelayCommand ShowDepartmentCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public MainViewModel(DashboardViewModel dashboardVm, EmployeeViewModel employeeVm, SalaryViewModel salaryVm,
                              AttendanceViewModel attendanceVm, DepartmentViewModel departmentVm, User currentUser)
        {
            DashboardVM = dashboardVm;
            EmployeeVM = employeeVm;
            SalaryVM = salaryVm;
            AttendanceVM = attendanceVm;
            DepartmentVM = departmentVm;
            CurrentUser = currentUser;

            ShowDashboardCommand = new RelayCommand(_ => { DashboardVM.LoadStats(); CurrentView = DashboardVM; });
            ShowEmployeeCommand = new RelayCommand(_ => CurrentView = EmployeeVM);
            ShowSalaryCommand = new RelayCommand(_ => CurrentView = SalaryVM);
            ShowAttendanceCommand = new RelayCommand(_ => CurrentView = AttendanceVM);
            ShowDepartmentCommand = new RelayCommand(_ => CurrentView = DepartmentVM);

            LogoutCommand = new RelayCommand(_ =>
            {
                var result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    RequestRestart?.Invoke();
                }
            });

            CurrentView = DashboardVM;
        }

        public System.Action RequestRestart { get; set; }
    }
}