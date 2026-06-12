using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Drive_Smart_2._0.Views.Auth
{
    public partial class EmployeeRegister : Window
    {
        public EmployeeRegister()
        {
            InitializeComponent();
        }

        // ==========================
        // Validation Methods
        // ==========================

        private bool IsValidEmployeeID()
        {
            return Regex.IsMatch(EmployeeID.Text, @"^EMP\d{4}$");
        }

        private bool IsValidNIC()
        {
            string oldNicPattern = @"^[0-9]{9}[VvXx]$";
            string newNicPattern = @"^[0-9]{12}$";

            return Regex.IsMatch(NIC.Text, oldNicPattern) ||
                   Regex.IsMatch(NIC.Text, newNicPattern);
        }

        private bool IsValidEmail()
        {
            try
            {
                MailAddress mail = new MailAddress(EmailAddress.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone()
        {
            return Regex.IsMatch(PhoneNumber.Text, @"^(07\d{8}|0\d{9})$");
        }

        private bool IsValidSalary()
        {
            return decimal.TryParse(Salary.Text, out decimal salary)
                   && salary >= 0;
        }

        private void SetFieldColor(TextBox textBox, bool isValid)
        {
            textBox.Background = isValid
                ? Brushes.LightGreen
                : Brushes.LightCoral;
        }

        // ==========================
        // Real-Time Validation
        // ==========================

        private void EmployeeID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmployeeID.Text))
            {
                EmployeeID.Background = Brushes.White;
                return;
            }

            SetFieldColor(EmployeeID, IsValidEmployeeID());
        }

        


        private void NIC_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NIC.Text))
            {
                NIC.Background = Brushes.White;
                return;
            }

            SetFieldColor(NIC, IsValidNIC());
        }

        private void EmailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailAddress.Text))
            {
                EmailAddress.Background = Brushes.White;
                return;
            }

            SetFieldColor(EmailAddress, IsValidEmail());
        }

        private void PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber.Text))
            {
                PhoneNumber.Background = Brushes.White;
                return;
            }

            SetFieldColor(PhoneNumber, IsValidPhone());
        }

        private void Salary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Salary.Text))
            {
                Salary.Background = Brushes.White;
                return;
            }

            SetFieldColor(Salary, IsValidSalary());
        }

        // ==========================
        // Reset Button
        // ==========================

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            EmployeeID.Clear();
            FullName.Clear();
            NIC.Clear();
            EmailAddress.Clear();
            PhoneNumber.Clear();
            Salary.Clear();
            Address.Clear();

            Gender.SelectedIndex = 0;
            Position.SelectedIndex = 0;

            DateOfBirth.SelectedDate = null;
            JoiningDate.SelectedDate = null;

            EmployeeID.Focus();
        }

        // ==========================
        // Register Button
        // ==========================

        private void Employee_Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmployeeID.Text))
            {
                MessageBox.Show("Employee ID is required.");
                EmployeeID.Focus();
                return;
            }

            if (!IsValidEmployeeID())
            {
                MessageBox.Show("Employee ID format should be like EMP0001");
                EmployeeID.Focus();
                EmployeeID.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(FullName.Text))
            {
                MessageBox.Show("Full Name is required.");
                FullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(NIC.Text))
            {
                MessageBox.Show("NIC is required.");
                NIC.Focus();
                return;
            }

            if (!IsValidNIC())
            {
                MessageBox.Show("Invalid NIC format.");
                NIC.Focus();
                NIC.SelectAll();
                return;
            }
            if (Gender.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a gender.");
                Gender.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailAddress.Text))
            {
                MessageBox.Show("Email Address is required.");
                EmailAddress.Focus();
                return;
            }

            if (!IsValidEmail())
            {
                MessageBox.Show("Invalid Email Address.");
                EmailAddress.Focus();
                EmailAddress.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber.Text))
            {
                MessageBox.Show("Phone Number is required.");
                PhoneNumber.Focus();
                return;
            }

            if (!IsValidPhone())
            {
                MessageBox.Show("Invalid Phone Number.");
                PhoneNumber.Focus();
                PhoneNumber.SelectAll();
                return;
            }

            if (DateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Please select Date Of Birth.");
                return;
            }
            if (Position.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a gender.");
                Gender.Focus();
                return;
            }

            int age = DateTime.Now.Year - DateOfBirth.SelectedDate.Value.Year;

            if (age < 18)
            {
                MessageBox.Show("Employee must be at least 18 years old.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Salary.Text))
            {
                MessageBox.Show("Salary is required.");
                Salary.Focus();
                return;
            }

            if (!IsValidSalary())
            {
                MessageBox.Show("Invalid Salary Amount.");
                Salary.Focus();
                Salary.SelectAll();
                return;
            }

            if (JoiningDate.SelectedDate == null)
            {
                MessageBox.Show("Please select Joining Date.");
                return;
            }

            if (DateOfBirth.SelectedDate >= JoiningDate.SelectedDate)
            {
                MessageBox.Show("Date Of Birth must be earlier than Joining Date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Address.Text))
            {
                MessageBox.Show("Address is required.");
                Address.Focus();
                return;
            }

            MessageBox.Show(
                "Employee Registered Successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}