using Drive_Smart_2._0.Views.VehicleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Drive_Smart_2._0.Views.VehicleView.Database;
using Microsoft.Data.Sqlite;
using System.Windows;

namespace Drive_Smart_2._0.models
{
    public partial class dashboard : Window
    {
        private void LoadVehicleTable()
        {
            var list = new List<Vehicle>();

            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Id, Brand, Model, PlateNumber, 
                           Year, Color, DailyRate, Status 
                    FROM Vehicles 
                    ORDER BY Status, Brand";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Vehicle
                    {
                        Id = reader.GetInt32(0),
                        Brand = reader.GetString(1),
                        Model = reader.GetString(2),
                        PlateNumber = reader.GetString(3),
                        Year = reader.GetInt32(4),
                        Color = reader.GetString(5),
                        DailyRate = reader.GetDouble(6),
                        Status = reader.GetString(7)
                    });
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show("Could not load vehicles:\n" + ex.Message);
            }

            dgVehicles.ItemsSource = list;
        }
        public dashboard()
        {
            InitializeComponent();
            VehicleDatabase.InitializeDatabase();
            LoadStats();
            LoadVehicleTable();
        }

        private void LoadStats()
        {
            using var conn = VehicleDatabase.GetConnection();
            conn.Open();

            // Total vehicles
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Vehicles";
            int total = Convert.ToInt32(cmd.ExecuteScalar());
            totvehecles.Text = total.ToString();

            // Available vehicles
            cmd.CommandText = "SELECT COUNT(*) FROM Vehicles WHERE Status = 'Available'";
            int available = Convert.ToInt32(cmd.ExecuteScalar());
            availablevehecletxt.Text = available.ToString();

            // Rented vehicles
            cmd.CommandText = "SELECT COUNT(*) FROM Vehicles WHERE Status = 'Rented'";
            int rented = Convert.ToInt32(cmd.ExecuteScalar());
            rentedvehecletxt.Text = rented.ToString();

            // Monthly revenue estimate (rented × avg daily rate × 30)
            cmd.CommandText = @"SELECT AVG(DailyRate) FROM Vehicles 
                                 WHERE Status = 'Rented'";
            var avg = cmd.ExecuteScalar();
            if (avg != DBNull.Value && avg != null)
            {
                double revenue = Convert.ToDouble(avg) * rented * 30;
                monthlyincometxt.Text = "Rs. " +
                    ((int)(revenue / 1000)).ToString() + "K";
            }
            else
            {
                monthlyincometxt.Text = "Rs. 0";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PublicVehicleView publicVehicleView = new PublicVehicleView();
            //publicVehicleView.Show();
            //this.Close();

            // Edited By amishka 
            DashboardWindowFrame.Navigate(new PublicVehicleView());
            DashboardWindowFrame.Visibility = Visibility.Visible;

        }
    }
}