using Drive_Smart_2._0.Data;
using Drive_Smart_2._0.models;
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


                //MessageBox.Show("Step 4");

            //---------------------------------------------------------
            // Amishka's code dont put your a@# here [starts here]   |
            //---------------------------------------------------------

            VehicleDatabase.InitializeDatabase();



                //PublicVehicleView window = new PublicVehicleView();
                //window.Show();

                //ChangePasswordWindow changePassword = new ChangePasswordWindow();
                //changePassword.Show();
                //
                login log = new login();
                log.Show();

                //EmployeeManagementt emp = new EmployeeManagementt();
                //emp.Show();

                //EmployeeRegister reg = new EmployeeRegister();
                //reg.Show();

                

            //---------------------------------------------------------
            // End of Amishka's Code                                 |
            //---------------------------------------------------------

            

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
