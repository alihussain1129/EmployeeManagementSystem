using EmployeeManagementSystem.Helpers;

namespace EmployeeManagementSystem.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        private string _errorMessage;

        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    }
}