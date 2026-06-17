using Drive_Smart_2._0.Views.Auth;
using Drive_Smart_2._0.Views.Auth.Helpers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Drive_Smart_2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLoggedEmployee();
            StartClock();
            ApplyPermissions();
        }





        // Load the logged-in employee's information into the sidebar
        private void LoadLoggedEmployee()
        {
            var emp = SessionManager.CurrentEmployee;

            if (emp == null)
            {
                TxtLoggedName.Text = "Guest";
                TxtLoggedRole.Text = "Unknown";
                TxtInitials.Text = "?";
                return;
            }

            // Employee Name
            TxtLoggedName.Text = emp.FullName;

            // Employee Role
            TxtLoggedRole.Text = emp.Position;

            // Welcome Message (Optional)
            TxtWelcome.Text = $"Welcome Back, {emp.FullName}";

            // Generate Initials
            var parts = emp.FullName.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                TxtInitials.Text =
                    $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }
            else if (parts.Length == 1)
            {
                TxtInitials.Text =
                    $"{parts[0][0]}".ToUpper();
            }
            else
            {
                TxtInitials.Text = "?";
            }

            // Role Badge Color
            switch (emp.Position)
            {
                case "Admin":
                    RoleBadge.Background = Brushes.MediumPurple;
                    TxtLoggedRole.Foreground = Brushes.White;
                    break;

                case "Manager":
                    RoleBadge.Background = Brushes.DarkOrange;
                    TxtLoggedRole.Foreground = Brushes.White;
                    break;

                case "Staff":
                    RoleBadge.Background = Brushes.DodgerBlue;
                    TxtLoggedRole.Foreground = Brushes.White;
                    break;

                default:
                    RoleBadge.Background = Brushes.Gray;
                    TxtLoggedRole.Foreground = Brushes.White;
                    break;
            }
        }
        // Clock Functionality
        private DispatcherTimer _timer;

        private void StartClock()
        {
            _timer = new DispatcherTimer();

            _timer.Interval =
                TimeSpan.FromSeconds(1);

            _timer.Tick += (s, e) =>
            {
                TxtDateTime.Text =
                    DateTime.Now.ToString(
                        "dddd, dd MMM yyyy | hh:mm:ss tt");
            };

            _timer.Start();
        }
        // Logout Button Click Event Handler
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Logout from Drive Smart 2.0?\n\n" +
                $"Logged in as: {SessionManager.CurrentEmployee?.FullName}",
                "Confirm Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            SessionManager.Logout();

            login loginWindow = new login();
            loginWindow.Show();

            Window.GetWindow(this)?.Close();
        }
        // Apply permissions based on employee role
        private void ApplyPermissions()
        {
            var emp = SessionManager.CurrentEmployee;

            if (emp == null)
                return;

            switch (emp.Position)
            {
                case "Admin":

                    // Full Access
                    break;

                case "Manager":

                    BtnEmployees.Visibility = Visibility.Collapsed;
                    //BtnSettings.Visibility = Visibility.Collapsed;

                    break;

                case "Staff":

                    BtnEmployees.Visibility = Visibility.Collapsed;
                    BtnPayments.Visibility = Visibility.Collapsed;
                    BtnReports.Visibility = Visibility.Collapsed;
                    //BtnSettings.Visibility = Visibility.Collapsed;

                    break;
            }
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentEmployee?.Position != "Admin")
            {
                MessageBox.Show(
                    "Access Denied",
                    "Permission Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EmployeeManagementt employeeWindow =
                new EmployeeManagementt();

            employeeWindow.Show();
        }
    }
}
