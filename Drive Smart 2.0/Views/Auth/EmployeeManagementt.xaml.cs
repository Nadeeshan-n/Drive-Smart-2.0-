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
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            addEmployee addEmployeeWindow = new addEmployee();
            addEmployeeWindow.Show();
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
    }
}
