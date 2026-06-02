using Npgsql;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class VehicleView_V2 : Window
    {
        public VehicleView_V2()
        {
            InitializeComponent();











            // Database connection

            string connString =
                "Host=ep-lucky-dust-ap0aolwl-pooler.c-7.us-east-1.aws.neon.tech; " +
                "Database=neondb; " +
                "Username=neondb_owner; " +
                "Password=npg_Sq6ek0IbhQFK; " +
                "SSL Mode=VerifyFull; " +
                "Channel Binding=Require;";


            try
            {
                using var conn = new NpgsqlConnection(connString);

                conn.Open();

                //MessageBox.Show("Connected Successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }











        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {             
            
        }
    
    }
}
