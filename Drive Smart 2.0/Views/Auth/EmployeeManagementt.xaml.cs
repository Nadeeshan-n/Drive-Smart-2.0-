using ClosedXML.Excel;
using Drive_Smart_2._0.Data;
using Drive_Smart_2._0.Models;
using Drive_Smart_2._0.Views.Auth.Helpers;
using Drive_Smart_2._0.UserControls;
using Microsoft.EntityFrameworkCore;
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


namespace Drive_Smart_2._0.Views.Auth
{
    /// <summary>
    /// Interaction logic for EmployeeManagementt.xaml
    /// </summary>
    public partial class EmployeeManagementt : Window
    {
        public EmployeeManagementt()
        {

            InitializeComponent();
            // Set the active page in the sidebar to Employees
            SidebarMenu.SetActivePage(ActivePage.Employees);
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //addEmployee addEmployeeWindow = new addEmployee();
            //addEmployeeWindow.Show();
        }


        //Load data
        private void LoadEmployees()
        {
            using (var db = new AppDbContext())
            {
                DgEmployees.ItemsSource = db.Employees.ToList();
            }
        }

        private void DgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee employee)
            {
                TxtEmployeeID.Text = employee.EmployeeID;
                TxtFullName.Text = employee.FullName;
                TxtNIC.Text = employee.NIC;
                TxtPhone.Text = employee.Phone;
                TxtEmail.Text = employee.Email;
                TxtAddress.Text = employee.Address;
                TxtSalary.Text = employee.Salary.ToString();

                CmbGender.Text = employee.Gender;
                CmbPosition.Text = employee.Position;
                DpDateOfBirth.SelectedDate = employee.DateOfBirth;
                DpJoiningDate.SelectedDate = employee.JoiningDate;
            }
        }

        private void txtBrand_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtBrand_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtBrand_Copy1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtBrand_Copy2_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle role selection change if needed
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            EmployeeRegister employeeRegister = new EmployeeRegister();
            employeeRegister.Show();
            this.Close();
        }
        // delete an employee
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee employee)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Delete employee {employee.FullName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var employeeToDelete =
                            db.Employees.FirstOrDefault(x => x.Id == employee.Id);

                        if (employeeToDelete != null)
                        {
                            db.Employees.Remove(employeeToDelete);
                            db.SaveChanges();
                        }
                    }

                    LoadEmployees();

                    MessageBox.Show(
                        "Employee deleted successfully.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select an employee first.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

        }
        //clear the text fields
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TxtEmployeeID.Clear();
            TxtFullName.Clear();
            TxtNIC.Clear();
            TxtSalary.Clear();
            TxtPhone.Clear();
            TxtEmail.Clear();
            TxtAddress.Clear();

            CmbGender.SelectedIndex = -1;
            CmbPosition.SelectedIndex = -1;
            DpDateOfBirth.SelectedDate = null;
            DpJoiningDate.SelectedDate = null;

            DgEmployees.SelectedItem = null;
        }
        //update employee details
        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            // ── Basic field validation ────────────────────────────
            if (string.IsNullOrWhiteSpace(TxtEmployeeID.Text) ||
                string.IsNullOrWhiteSpace(TxtFullName.Text) ||
                string.IsNullOrWhiteSpace(TxtNIC.Text) ||
                string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                MessageBox.Show(
                    "Employee ID, Full Name, NIC and Phone are required.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                // ── Check Employee ID already used by another employee ──
                bool employeeIDExists = db.Employees.Any(x =>
                    x.EmployeeID == TxtEmployeeID.Text.Trim() &&
                    x.Id != selectedEmployee.Id);  // exclude current employee

                if (employeeIDExists)
                {
                    MessageBox.Show(
                        $"Employee ID '{TxtEmployeeID.Text.Trim()}' is already " +
                        $"assigned to another employee.",
                        "Duplicate Employee ID",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                //  Check NIC already used by another employee
                bool nicExists = db.Employees.Any(x =>
                    x.NIC == TxtNIC.Text.Trim() &&
                    x.Id != selectedEmployee.Id);  // exclude current employee

                if (nicExists)
                {
                    MessageBox.Show(
                        $"NIC '{TxtNIC.Text.Trim()}' is already " +
                        $"registered to another employee.",
                        "Duplicate NIC",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Check Phone already used by another employee
                bool phoneExists = db.Employees.Any(x =>
                    x.Phone == TxtPhone.Text.Trim() &&
                    x.Id != selectedEmployee.Id);  // exclude current employee

                if (phoneExists)
                {
                    MessageBox.Show(
                        $"Phone number '{TxtPhone.Text.Trim()}' is already " +
                        $"registered to another employee.",
                        "Duplicate Phone Number",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                bool emailExists = db.Employees.Any(x =>
                    x.Email == TxtEmail.Text.Trim() &&
                    x.Id != selectedEmployee.Id);  // exclude current employee

                if (emailExists)
                {
                    MessageBox.Show(
                        $"Email '{TxtEmail.Text.Trim()}' is already " +
                        $"registered to another employee.",
                        "Duplicate Email",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // ── All checks passed — update the employee 
                var employee = db.Employees
                                 .FirstOrDefault(x => x.Id == selectedEmployee.Id);

                if (employee != null)
                {
                    employee.EmployeeID = TxtEmployeeID.Text.Trim();
                    employee.FullName = TxtFullName.Text.Trim();
                    employee.NIC = TxtNIC.Text.Trim();
                    employee.Gender = CmbGender.Text;
                    employee.Position = CmbPosition.Text;
                    employee.Phone = TxtPhone.Text.Trim();
                    employee.Email = TxtEmail.Text.Trim();
                    employee.Address = TxtAddress.Text.Trim();

                    if (decimal.TryParse(TxtSalary.Text, out decimal salary))
                        employee.Salary = salary;

                    employee.DateOfBirth = DpDateOfBirth.SelectedDate ?? DateTime.Now;
                    employee.JoiningDate = DpJoiningDate.SelectedDate ?? DateTime.Now;

                    db.SaveChanges();

                    LoadEmployees();

                    MessageBox.Show(
                        "Employee updated successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }
        //Search for an employee by ID or name

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {

            
            string searchText = TxtSearch.Text.Trim();

            using (var db = new AppDbContext())
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    DgEmployees.ItemsSource = db.Employees.ToList();
                    return;
                }

                var employees = db.Employees
                    .Where(emp =>
                        emp.EmployeeID.ToLower().Contains(searchText) ||
                        emp.FullName.ToLower().Contains(searchText))
                    .ToList();

                DgEmployees.ItemsSource = employees;
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            using (var db = new AppDbContext())
            {
                DgEmployees.ItemsSource = db.Employees
                    .Where(emp =>
                        emp.EmployeeID.Contains(searchText) ||
                        emp.FullName.Contains(searchText))
                    .ToList();
            }
        }
        //Sort employees by selected criteria
        private void CmbSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (CmbSortBy.SelectedItem == null)
                return;

            if (!IsLoaded)
                return;

            string sortOption =
                (CmbSortBy.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            using (var db = new AppDbContext())
            {
                switch (sortOption)
                {
                    case "Employee ID":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderBy(emp => emp.EmployeeID)
                            .ToList();
                        break;

                    case "Full Name":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderBy(emp => emp.FullName)
                            .ToList();
                        break;

                    case "Position":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderBy(emp => emp.Position)
                            .ToList();
                        break;

                    /*case "Salary (Low to High)":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderBy(emp => emp.Salary)
                            .ToList();
                        break;

                    case "Salary (High to Low)":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderByDescending(emp => emp.Salary)
                            .ToList();
                        break;*/

                    case "Joining Date":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderBy(emp => emp.JoiningDate)
                            .ToList();
                        break;

                    default:
                        LoadEmployees();
                        break;
                }
            }
        }
        //reset employee password
        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }
            /*if (selectedEmployee.EmployeeID == SessionManager.CurrentEmployee?.EmployeeID)
            {
                MessageBox.Show(
                    "You cannot reset your own password here.\n" +
                    "Please use the Change Password option instead.",
                    "Not Allowed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }*/

            var confirm = MessageBox.Show(
                $"Reset password for {selectedEmployee.FullName}?\n\n" +
                $"A new temporary password will be generated.\n" +
                $"The employee will be required to change it on next login.",
                "Confirm Password Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = new AppDbContext())
                {
                    var employee = db.Employees
                        .FirstOrDefault(x =>
                            x.EmployeeID == selectedEmployee.EmployeeID);
                    var username = employee?.Username ?? "Unknown";


                    if (employee == null)
                    {
                        MessageBox.Show(
                            "Employee not found.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    // Generate default password
                    string nicPart =
                        employee.NIC.Length >= 4
                        ? employee.NIC.Substring(0, 4)
                        : employee.NIC;

                    string rawPassword =
                        $"DS@{employee.EmployeeID}@{nicPart}";

                    // Hash password
                    employee.PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(rawPassword);

                    // Force password change on next login
                    employee.MustChangePassword = true;

                    db.SaveChanges();

                    MessageBox.Show(
                        $"Password reset successfully.\n\n" +
                        $"Username: {username}\n" +
                        $"Temporary Password:\n{rawPassword}\n\n" +
                        $"The employee must change this password at the next login.",
                        "Password Reset Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error resetting password:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }
    }
}
