using Drive_Smart_2._0.models;
using Drive_Smart_2._0.Views.Auth;
using Drive_Smart_2._0.Views.VehicleView;
using Drive_Smart_2._0.Views.VehicleView.Database;
using Drive_Smart_2._0.Views.Payment;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Drive_Smart_2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            
            //base.OnStartup(e);
           // var dash = new EmployeeRegister();
            //dash.Show();


            //---------------------------------------------------------
            // Amishka's code dont put your a@# here [starts here]   |
            //---------------------------------------------------------

            VehicleDatabase.InitializeDatabase();

            //PublicVehicleView window = new PublicVehicleView();
            //window.Show();
            
            //AdminVehicleView adminVehicleView = new AdminVehicleView();
            //adminVehicleView.Show();

            //---------------------------------------------------------
            // End of Amishka's Code                                 |
            //---------------------------------------------------------


            payment_details pay =  new payment_details();
            pay.Show();

            dashboard dashboardWindow = new dashboard();
            dashboardWindow.Show();

        }

    }

}
