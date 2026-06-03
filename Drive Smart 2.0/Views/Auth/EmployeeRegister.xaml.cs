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
using System.Net.Mail;

namespace Drive_Smart_2._0.Views.Auth
{
    /// <summary>
    /// Interaction logic for EmployeeRegister.xaml
    /// </summary>
    public partial class EmployeeRegister : Window
    {
        public EmployeeRegister()
        {
            InitializeComponent();
        }

        private void EmployeeID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NIC_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EmailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            EmployeeID.Clear();
            FullName.Clear();
            NIC.Clear();
            Gender.SelectedIndex = 0;
            EmailAddress.Clear();
            PhoneNumber.Clear();
            DateOfBirth.SelectedDate = null;
            Posision_Copy1.Clear();
            JoiningDate.SelectedDate = null;
            Address.Clear();
        }

        private void Employee_Register_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
