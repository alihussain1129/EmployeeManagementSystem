using System.Windows;
using System.Windows.Input;
using EmployeeManagementSystem.ViewModels;

namespace EmployeeManagementSystem.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseAction = () => DialogResult = true;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (LoginViewModel)DataContext;
            if (vm.LoginCommand.CanExecute(PasswordBox.Password))
                vm.LoginCommand.Execute(PasswordBox.Password);
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(sender, e);
        }
    }
}