using Drive_Smart_2._0.models;
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
            base.OnStartup(e);
            payment_details pay =  new payment_details();
            pay.Show();
        }

    }

}
