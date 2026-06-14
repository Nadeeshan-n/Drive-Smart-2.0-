using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Drive_Smart_2._0.Data;
using Drive_Smart_2._0.Views.Auth.Helpers;
using BCrypt.Net;

namespace Drive_Smart_2._0.Views.Auth
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
            loginPassword.KeyDown += LoginPassword_KeyDown;
        }


        private void LoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameBox.Text.Trim();
            string password = loginPassword.Password;

            TxtError.Visibility = Visibility.Collapsed;

            // Validation
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                TxtError.Text = "Please enter both username and password.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var employee = db.Employees
                    .FirstOrDefault(emp =>
                        emp.Username == username &&
                        emp.IsApproved);

                if (employee == null)
                {
                    TxtError.Text = "Invalid username or password.";
                    TxtError.Visibility = Visibility.Visible;
                    loginPassword.Clear();
                    return;
                }

                bool passwordValid =
                    BCrypt.Net.BCrypt.Verify(
                        password,
                        employee.PasswordHash);

                if (!passwordValid)
                {
                    TxtError.Text = "Invalid username or password.";
                    TxtError.Visibility = Visibility.Visible;
                    loginPassword.Clear();
                    return;
                }

                // Save logged-in employee
                SessionManager.CurrentEmployee = employee;

                // Force password change if required
                if (employee.MustChangePassword)
                {
                    ChangePasswordWindow changeWindow =
                        new ChangePasswordWindow();

                    bool? result =
                        changeWindow.ShowDialog();

                    if (result != true)
                    {
                        SessionManager.Logout();
                        return;
                    }
                }

                //MainWindow mainWindow = new MainWindow();
                //mainWindow.Show();
                EmployeeRegister employeeRegister = new EmployeeRegister();
                employeeRegister.Show();

                EmployeeManagementt employeeManagementt = new EmployeeManagementt();
                employeeManagementt.Show();

                Close();
            }
            catch (Exception ex)
            {
                TxtError.Text =
                    "Unable to connect to the database.";
                TxtError.Visibility = Visibility.Visible;

                MessageBox.Show(
                    ex.Message,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
