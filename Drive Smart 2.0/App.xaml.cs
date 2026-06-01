using Drive_Smart_2._0.models;
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
            var dash = new Window1();
            dash.Show();
        }
    }

}
