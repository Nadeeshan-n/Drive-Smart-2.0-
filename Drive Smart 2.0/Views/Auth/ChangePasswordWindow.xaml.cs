using Drive_Smart_2._0.Data;
using Drive_Smart_2._0.Views.Auth.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Drive_Smart_2._0.Views.Auth
{
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();

            this.Closing += (s, e) =>
            {
                if (SessionManager.CurrentEmployee != null &&
                    SessionManager.CurrentEmployee.MustChangePassword)
                {
                    MessageBox.Show(
                        "You must change your password to continue.",
                        "Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    e.Cancel = true;
                }
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = PwdNew.Password;
            string confirmPassword = PwdConfirm.Password;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show(
                    "Please enter a new password.",
                    "Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show(
                    "Password must be at least 6 characters.",
                    "Too Short",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            

            if (newPassword.StartsWith("DS@"))
            {
                MessageBox.Show(
                    "Please choose a different password.",
                    "Weak Password",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(
                    newPassword,
                    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                MessageBox.Show(
                    "Password must contain:\n\n" +
                    "• At least 8 characters\n" +
                    "• One uppercase letter\n" +
                    "• One lowercase letter\n" +
                    "• One number\n" +
                    "• One special character",
                    "Weak Password",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show(
                    "Passwords do not match.",
                    "Mismatch",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            using var db = new AppDbContext();

            var emp = db.Employees.FirstOrDefault(x =>
                x.EmployeeID ==
                SessionManager.CurrentEmployee!.EmployeeID);

            if (emp == null)
            {
                MessageBox.Show(
                    "Employee record not found.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            emp.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newPassword);

            emp.MustChangePassword = false;

            db.SaveChanges();

            SessionManager.CurrentEmployee.MustChangePassword = false;

            MessageBox.Show(
                "Password changed successfully!\n" +
                "You can now access the system.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
