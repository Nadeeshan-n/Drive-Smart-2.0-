using Drive_Smart_2._0.Views.Auth;
using Drive_Smart_2._0.Views.Auth.Helpers;
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


namespace Drive_Smart_2._0.UserControls
{
    /// <summary>
    /// Interaction logic for SidebarControl.xaml
    /// </summary>
    public partial class SidebarControl : UserControl
    {
        public SidebarControl()
        {
            InitializeComponent();
            LoadLoggedEmployee();
        }


        private void LoadLoggedEmployee()
        {
            // Load the logged-in employee's information from the SessionManager
            var emp = SessionManager.CurrentEmployee;
            if (emp == null)
                return;

            TxtLoggedName.Text = emp.FullName;
            TxtLoggedRole.Text = emp.Position;

            var parts = emp.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            //name first letter of first and last name, if only one name, just first letter of that name

            TxtInitials.Text = parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : $"{parts[0][0]}".ToUpper();
            RoleBadge.Background = emp.Position switch
            {
                "Admin" => Brushes.Purple,
                "Manager" => Brushes.DarkOrange,
                _ => Brushes.DodgerBlue
            };
        }

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
    }
}
