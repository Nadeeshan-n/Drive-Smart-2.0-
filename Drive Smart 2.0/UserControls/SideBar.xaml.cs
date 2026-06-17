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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Drive_Smart_2._0.models;
using Drive_Smart_2._0.Models;
using Drive_Smart_2._0.Views.Auth;
using Drive_Smart_2._0.Views.Auth.Helpers;
using Drive_Smart_2._0.Views.Customer;
using Drive_Smart_2._0.Views.Payment;
using Drive_Smart_2._0.Views.Reports;
using Drive_Smart_2._0.Views.VehicleView;



namespace Drive_Smart_2._0.UserControls
{
    /// <summary>
    /// Interaction logic for SideBar.xaml
    /// </summary>
    public partial class SideBar : UserControl
    {
        public SideBar()
        {
            InitializeComponent();
            ApplyPermissions();
        }

    public void SetActivePage(ActivePage page)
    {
        Brush normalColor =
            new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#1565C0"));

        Brush activeColor =
            new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString("#1E88E5"));

    // Reset all buttons
        BtnDashboard.Background = normalColor;
        BtnVehicles.Background = normalColor;
        BtnCustomers.Background = normalColor;
        BtnPayments.Background = normalColor;
        BtnReports.Background = normalColor;
        BtnEmployees.Background = normalColor;
        BtnHome.Background = normalColor;

    // Highlight current page
        switch (page)
        {
            case ActivePage.Dashboard:
                BtnDashboard.Background = activeColor;
                break;

            case ActivePage.Vehicles:
                BtnVehicles.Background = activeColor;
                break;

            case ActivePage.Customers:
                BtnCustomers.Background = activeColor;
                break;

            case ActivePage.VehicleRegister:
                BtnVehiclesRegister.Background = activeColor;
                break;


            case ActivePage.VehicleMaintenance:
                BtnVehicleMaintenance.Background = activeColor;
                break;

            case ActivePage.Payments:
                BtnPayments.Background = activeColor;
                break;

            case ActivePage.Reports:
                BtnReports.Background = activeColor;
                break;

            case ActivePage.Employees:
                BtnEmployees.Background = activeColor;
                break;

            case ActivePage.Home:
                BtnHome.Background = activeColor;
                break;
        }


    }
        private void ApplyPermissions()
        {
            var employee = SessionManager.CurrentEmployee;

            if (employee == null)
                return;

            switch (employee.Position)
            {
                case "Admin":

                    // Full access
                    break;

                case "Manager":

                    BtnEmployees.Visibility =
                        Visibility.Collapsed;

                    

                    break;

                case "Staff":

                    BtnEmployees.Visibility =
                        Visibility.Collapsed;

                    BtnPayments.Visibility =
                        Visibility.Collapsed;

                    BtnReports.Visibility =
                        Visibility.Collapsed;

                    BtnVehicleMaintenance.Visibility = Visibility.Collapsed;
                    BtnVehiclesRegister.Visibility = Visibility.Collapsed;

                    

                    break;
                }
            }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            dashboard dashboard = new dashboard();
            dashboard.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnVehicles_Click(object sender, RoutedEventArgs e)
        {
            PublicVehicleView vehicleView = new PublicVehicleView();
            vehicleView.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnVehiclesRegister_Click(object sender, RoutedEventArgs e)
        {
            AdminVehicleView register = new AdminVehicleView();
            register.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnVehicleMaintenance_Click(object sender, RoutedEventArgs e)
        {
            MaintenanceView maintenance = new MaintenanceView();
            maintenance.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();
            customer.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnPayments_Click(object sender, RoutedEventArgs e)
        {
            payment_details pay = new payment_details();
            pay.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            reports report = new reports();
            report.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManagement emp = new EmployeeManagement();
            emp.Show();
            Window.GetWindow(this)?.Close();
        }
    }
    
}
