using System;
using System.Collections.ObjectModel;
using System.Windows;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.ViewModels
{
    public class DepartmentViewModel : BaseViewModel
    {
        private readonly DepartmentService _service;
        private readonly string _currentUserRole;

        public bool IsAdmin => _currentUserRole == "Admin";

        public ObservableCollection<Department> Departments { get; set; }

        private Department _selectedDepartment;
        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged();
                DepartmentName = value?.Name ?? string.Empty;
            }
        }

        private string _departmentName = string.Empty;
        public string DepartmentName
        {
            get => _departmentName;
            set { _departmentName = value; OnPropertyChanged(); }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }

        public DepartmentViewModel(DepartmentService service, string currentUserRole)
        {
            _service = service;
            _currentUserRole = currentUserRole;
            Departments = new ObservableCollection<Department>();

            AddCommand = new RelayCommand(_ => AddDepartment());
            UpdateCommand = new RelayCommand(_ => UpdateDepartment(), _ => SelectedDepartment != null);
            DeleteCommand = new RelayCommand(_ => DeleteDepartment(), _ => SelectedDepartment != null && IsAdmin);
            ClearCommand = new RelayCommand(_ => ClearForm());

            LoadDepartments();
        }

        private void LoadDepartments()
        {
            Departments.Clear();
            foreach (var dept in _service.GetAllDepartments())
                Departments.Add(dept);
        }

        private void AddDepartment()
        {
            try
            {
                _service.AddDepartment(new Department { Name = DepartmentName });
                LoadDepartments();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDepartment()
        {
            if (SelectedDepartment == null) return;

            try
            {
                SelectedDepartment.Name = DepartmentName;
                _service.UpdateDepartment(SelectedDepartment);
                LoadDepartments();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;

            if (!IsAdmin)
            {
                MessageBox.Show("Only Admin users can delete departments.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Delete department '{SelectedDepartment.Name}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _service.DeleteDepartment(SelectedDepartment.DepartmentId);
                LoadDepartments();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            SelectedDepartment = null;
            DepartmentName = string.Empty;
        }
    }
}