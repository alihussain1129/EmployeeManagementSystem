using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.ViewModels
{
    public class EmployeeViewModel : BaseViewModel
    {
        private readonly EmployeeService _employeeService;
        private readonly DepartmentService _departmentService;
        private readonly string _currentUserRole;

        public bool IsAdmin => _currentUserRole == "Admin";

        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Department> Departments { get; set; }
        public ICollectionView EmployeesView { get; }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                LoadFormFromSelection();
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                EmployeesView.Refresh();
            }
        }

        private string _fullName = "";
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }

        private string _cnic = "";
        public string CNIC { get => _cnic; set { _cnic = value; OnPropertyChanged(); } }

        private string _designation = "";
        public string Designation { get => _designation; set { _designation = value; OnPropertyChanged(); } }

        private Department _selectedDepartment;
        public Department SelectedDepartment { get => _selectedDepartment; set { _selectedDepartment = value; OnPropertyChanged(); } }

        private DateTime _joiningDate = DateTime.Today;
        public DateTime JoiningDate { get => _joiningDate; set { _joiningDate = value; OnPropertyChanged(); } }

        private string _biometricId = "";
        public string BiometricId { get => _biometricId; set { _biometricId = value; OnPropertyChanged(); } }

        private string _status = "Active";
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand ExportCommand { get; }

        public EmployeeViewModel(EmployeeService employeeService, DepartmentService departmentService, string currentUserRole)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _currentUserRole = currentUserRole;

            Employees = new ObservableCollection<Employee>();
            Departments = new ObservableCollection<Department>();

            EmployeesView = CollectionViewSource.GetDefaultView(Employees);
            EmployeesView.Filter = FilterEmployees;

            AddCommand = new RelayCommand(_ => AddEmployee());
            UpdateCommand = new RelayCommand(_ => UpdateEmployee(), _ => SelectedEmployee != null);
            DeleteCommand = new RelayCommand(_ => DeleteEmployee(), _ => SelectedEmployee != null && IsAdmin);
            ClearCommand = new RelayCommand(_ => ClearForm());
            ExportCommand = new RelayCommand(_ => ExportToCsv());

            LoadDepartments();
            LoadEmployees();
        }

        private bool FilterEmployees(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (obj is not Employee emp) return false;

            string term = SearchText.Trim();
            return (emp.FullName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (emp.CNIC?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (emp.Designation?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (emp.DepartmentName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (emp.Status?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private void LoadDepartments()
        {
            Departments.Clear();
            foreach (var d in _departmentService.GetAllDepartments())
                Departments.Add(d);
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var e in _employeeService.GetAllEmployees())
                Employees.Add(e);
        }

        private void LoadFormFromSelection()
        {
            if (SelectedEmployee == null) return;

            FullName = SelectedEmployee.FullName;
            CNIC = SelectedEmployee.CNIC;
            Designation = SelectedEmployee.Designation;
            SelectedDepartment = Departments.FirstOrDefault(d => d.DepartmentId == SelectedEmployee.DepartmentId);
            JoiningDate = SelectedEmployee.JoiningDate;
            BiometricId = SelectedEmployee.BiometricId;
            Status = SelectedEmployee.Status;
        }

        private void AddEmployee()
        {
            try
            {
                var emp = BuildEmployeeFromForm();
                _employeeService.AddEmployee(emp);
                LoadEmployees();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateEmployee()
        {
            if (SelectedEmployee == null) return;

            try
            {
                var emp = BuildEmployeeFromForm();
                emp.EmployeeId = SelectedEmployee.EmployeeId;
                _employeeService.UpdateEmployee(emp);
                LoadEmployees();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            if (!IsAdmin)
            {
                MessageBox.Show("Only Admin users can delete employees.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Delete employee '{SelectedEmployee.FullName}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _employeeService.DeleteEmployee(SelectedEmployee.EmployeeId);
                LoadEmployees();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToCsv()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"Employees_{DateTime.Today:yyyy-MM-dd}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var itemsToExport = EmployeesView.Cast<Employee>().ToList();
                    _employeeService.ExportToCsv(dialog.FileName, itemsToExport);
                    MessageBox.Show("Export completed successfully.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Employee BuildEmployeeFromForm()
        {
            return new Employee
            {
                FullName = FullName,
                CNIC = CNIC,
                Designation = Designation,
                DepartmentId = SelectedDepartment?.DepartmentId ?? 0,
                JoiningDate = JoiningDate,
                BiometricId = BiometricId,
                Status = Status
            };
        }

        private void ClearForm()
        {
            SelectedEmployee = null;
            FullName = "";
            CNIC = "";
            Designation = "";
            SelectedDepartment = null;
            JoiningDate = DateTime.Today;
            BiometricId = "";
            Status = "Active";
        }
    }
}