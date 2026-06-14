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
            try
            {
                base.OnStartup(e);

                //MessageBox.Show("Step 1");

                VehicleDatabase.InitializeDatabase();

                //MessageBox.Show("Step 2");
                using (var db = new AppDbContext())
                {
                    db.Database.EnsureCreated();
                }

                //MessageBox.Show("Step 3");

                //EmployeeRegister employeeRegister = new EmployeeRegister();
                //employeeRegister.Show();

                //MessageBox.Show("Step 4");

                // new MainWindow().Show();

                EmployeeManagementt emplyoeemanagement = new EmployeeManagementt();
                emplyoeemanagement.Show();

                //ChangePasswordWindow changePassword = new ChangePasswordWindow();
                //changePassword.Show();


            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
