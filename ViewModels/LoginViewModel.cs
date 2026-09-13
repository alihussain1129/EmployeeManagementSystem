using System;
using System.Windows;
using EmployeeManagementSystem.Helpers;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        public User LoggedInUser { get; private set; }
        public bool LoginSucceeded { get; private set; }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _errorText = string.Empty;
        public string ErrorText
        {
            get => _errorText;
            set { _errorText = value; OnPropertyChanged(); }
        }

        public RelayCommand LoginCommand { get; }

        public Action CloseAction { get; set; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(param => AttemptLogin(param as string));
        }

        private void AttemptLogin(string password)
        {
            ErrorText = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText = "Username and password are required.";
                return;
            }

            var user = _authService.Login(Username.Trim(), password);

            if (user == null)
            {
                ErrorText = "Invalid username or password.";
                return;
            }

            LoggedInUser = user;
            LoginSucceeded = true;
            CloseAction?.Invoke();
        }
    }
}