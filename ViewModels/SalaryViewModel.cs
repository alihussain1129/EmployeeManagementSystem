using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.ViewModels
{
    public class SalaryViewModel : BaseViewModel
    {
        private readonly SalaryService _salaryService;
        private readonly EmployeeService _employeeService;
        private readonly string _currentUserRole;

        public bool IsAdmin => _currentUserRole == "Admin";

        public ObservableCollection<Salary> Salaries { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }

        private Salary _selectedSalary;
        public Salary SelectedSalary
        {
            get => _selectedSalary;
            set { _selectedSalary = value; OnPropertyChanged(); LoadFormFromSelection(); }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        private decimal _basicSalary;
        public decimal BasicSalary { get => _basicSalary; set { _basicSalary = value; OnPropertyChanged(); } }

        private decimal _allowances;
        public decimal Allowances { get => _allowances; set { _allowances = value; OnPropertyChanged(); } }

        private decimal _deductions;
        public decimal Deductions { get => _deductions; set { _deductions = value; OnPropertyChanged(); } }

        private DateTime _effectiveMonth = DateTime.Today;
        public DateTime EffectiveMonth { get => _effectiveMonth; set { _effectiveMonth = value; OnPropertyChanged(); } }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }

        public SalaryViewModel(SalaryService salaryService, EmployeeService employeeService, string currentUserRole)
        {
            _salaryService = salaryService;
            _employeeService = employeeService;
            _currentUserRole = currentUserRole;

            Salaries = new ObservableCollection<Salary>();
            Employees = new ObservableCollection<Employee>();

            AddCommand = new RelayCommand(_ => AddSalary(), _ => SelectedEmployee != null);
            UpdateCommand = new RelayCommand(_ => UpdateSalary(), _ => SelectedSalary != null);
            DeleteCommand = new RelayCommand(_ => DeleteSalary(), _ => SelectedSalary != null && IsAdmin);
            ClearCommand = new RelayCommand(_ => ClearForm());

            LoadEmployees();
            LoadSalaries();
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var e in _employeeService.GetAllEmployees())
                Employees.Add(e);
        }

        private void LoadSalaries()
        {
            Salaries.Clear();
            foreach (var s in _salaryService.GetAllSalaries())
                Salaries.Add(s);
        }

        private void LoadFormFromSelection()
        {
            if (SelectedSalary == null) return;

            SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeId == SelectedSalary.EmployeeId);
            BasicSalary = SelectedSalary.BasicSalary;
            Allowances = SelectedSalary.Allowances;
            Deductions = SelectedSalary.Deductions;
            EffectiveMonth = SelectedSalary.EffectiveMonth;
        }

        private void AddSalary()
        {
            try
            {
                _salaryService.AddSalary(BuildFromForm());
                LoadSalaries();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSalary()
        {
            if (SelectedSalary == null) return;

            try
            {
                var salary = BuildFromForm();
                salary.SalaryId = SelectedSalary.SalaryId;
                _salaryService.UpdateSalary(salary);
                LoadSalaries();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSalary()
        {
            if (SelectedSalary == null) return;

            if (!IsAdmin)
            {
                MessageBox.Show("Only Admin users can delete salary records.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Delete this salary record?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _salaryService.DeleteSalary(SelectedSalary.SalaryId);
                LoadSalaries();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Salary BuildFromForm()
        {
            return new Salary
            {
                EmployeeId = SelectedEmployee?.EmployeeId ?? 0,
                BasicSalary = BasicSalary,
                Allowances = Allowances,
                Deductions = Deductions,
                EffectiveMonth = EffectiveMonth
            };
        }

        private void ClearForm()
        {
            SelectedSalary = null;
            SelectedEmployee = null;
            BasicSalary = 0;
            Allowances = 0;
            Deductions = 0;
            EffectiveMonth = DateTime.Today;
        }
    }
}