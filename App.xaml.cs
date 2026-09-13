using System.Windows;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Data.Repositories;
using EmployeeManagementSystem.Services;
using EmployeeManagementSystem.ViewModels;
using EmployeeManagementSystem.Views;

namespace EmployeeManagementSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var context = new DatabaseContext();

            IEmployeeRepository employeeRepo = new EmployeeRepository(context);
            ISalaryRepository salaryRepo = new SalaryRepository(context);
            IAttendanceRepository attendanceRepo = new AttendanceRepository(context);
            IDepartmentRepository departmentRepo = new DepartmentRepository(context);
            IUserRepository userRepo = new UserRepository(context);

            var employeeService = new EmployeeService(employeeRepo);
            var salaryService = new SalaryService(salaryRepo);
            var attendanceService = new AttendanceService(attendanceRepo, employeeRepo);
            var departmentService = new DepartmentService(departmentRepo);
            var authService = new AuthService(userRepo);
            var biometricListener = new BiometricListenerService(attendanceService);
            biometricListener.Start();

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            RunSessionLoop(employeeService, salaryService, attendanceService, departmentService, authService);
        }

        private void RunSessionLoop(EmployeeService employeeService, SalaryService salaryService,
                                     AttendanceService attendanceService, DepartmentService departmentService,
                                     AuthService authService)
        {
            while (true)
            {
                var loginVm = new LoginViewModel(authService);
                var loginView = new LoginView(loginVm);
                bool? loginResult = loginView.ShowDialog();

                if (loginResult != true || !loginVm.LoginSucceeded)
                {
                    Shutdown();
                    return;
                }

                string role = loginVm.LoggedInUser.Role;

                var dashboardVm = new DashboardViewModel(employeeService, departmentService, attendanceService);
                var employeeVm = new EmployeeViewModel(employeeService, departmentService, role);
                var salaryVm = new SalaryViewModel(salaryService, employeeService, role);
                var attendanceVm = new AttendanceViewModel(attendanceService, employeeService, role);
                var departmentVm = new DepartmentViewModel(departmentService, role);

                var mainVm = new MainViewModel(dashboardVm, employeeVm, salaryVm, attendanceVm, departmentVm, loginVm.LoggedInUser);

                var mainWindow = new MainWindow
                {
                    DataContext = mainVm
                };

                bool loggedOut = false;
                mainVm.RequestRestart = () =>
                {
                    loggedOut = true;
                    mainWindow.Close();
                };

                MainWindow = mainWindow;
                mainWindow.ShowDialog();

                if (!loggedOut)
                {
                    Shutdown();
                    return;
                }
            }
        }
    }
}