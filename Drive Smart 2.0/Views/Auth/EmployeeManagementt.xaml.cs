using ClosedXML.Excel;
using Drive_Smart_2._0.Data;
using Drive_Smart_2._0.Models;
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
using Microsoft.EntityFrameworkCore;
using System.Windows;


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
            //LoadEmployees();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            addEmployee addEmployeeWindow = new addEmployee();
            addEmployeeWindow.Show();
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

            using (var db = new AppDbContext())
            {
                var employee = db.Employees
                                 .FirstOrDefault(x => x.Id == selectedEmployee.Id);

                if (employee != null)
                {
                    employee.EmployeeID = TxtEmployeeID.Text;
                    employee.FullName = TxtFullName.Text;
                    employee.NIC = TxtNIC.Text;
                    employee.Gender = CmbGender.Text;
                    employee.Position = CmbPosition.Text;
                    employee.Phone = TxtPhone.Text;
                    employee.Email = TxtEmail.Text;
                    employee.Address = TxtAddress.Text;

                    if (decimal.TryParse(TxtSalary.Text, out decimal salary))
                        employee.Salary = salary;

                    employee.DateOfBirth = DpDateOfBirth.SelectedDate ?? DateTime.Now;
                    employee.JoiningDate = DpJoiningDate.SelectedDate ?? DateTime.Now;

                    db.SaveChanges();
                }
            }

            LoadEmployees();

            MessageBox.Show(
                "Employee updated successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
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

                    case "Salary (Low to High)":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderBy(emp => emp.Salary)
                            .ToList();
                        break;

                    case "Salary (High to Low)":
                        DgEmployees.ItemsSource = db.Employees
                            .OrderByDescending(emp => emp.Salary)
                            .ToList();
                        break;

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
    }
}
