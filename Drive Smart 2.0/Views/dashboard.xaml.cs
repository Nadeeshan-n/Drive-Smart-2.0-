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
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            VehicleDatabase.InitializeDatabase();
            LoadStats();
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
    }
}