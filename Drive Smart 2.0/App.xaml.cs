using Drive_Smart_2._0.Data;
using Drive_Smart_2._0.Views.Auth;
using Drive_Smart_2._0.Views.VehicleView.Database;
using System.Windows;

namespace Drive_Smart_2._0
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EmployeeRegister employeeRegister = new EmployeeRegister();
            employeeRegister.Show();

            VehicleDatabase.InitializeDatabase();

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            new MainWindow().Show();
        }
    }
}
