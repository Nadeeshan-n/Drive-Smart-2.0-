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

namespace Drive_Smart_2._0.Views.VehicleView
{
    /// <summary>
    /// Interaction logic for VVMainWindow.xaml
    /// </summary>
    public partial class VVMainWindow : Window
    {
        public VVMainWindow()
        {
            InitializeComponent();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            //   Start the admin vehicle view
            VVMainWindowFrame.Navigate(new AdminVehicleView());
            VVMainWindowFrame.Visibility = Visibility.Visible;
        }


        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            //   Start the admin vehicle view
            VVMainWindowFrame.Navigate(new PublicVehicleView());
            VVMainWindowFrame.Visibility = Visibility.Visible;
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            //   Start the admin vehicle view
            VVMainWindowFrame.Navigate(new AdminHelpView());
            VVMainWindowFrame.Visibility = Visibility.Visible;
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            VVMainWindowFrame.Navigate(new MaintenanceView());
            VVMainWindowFrame.Visibility = Visibility.Visible;

            
        }
    }
}
