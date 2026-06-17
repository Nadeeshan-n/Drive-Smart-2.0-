using Drive_Smart_2._0.Views.Auth.Helpers;
using Drive_Smart_2._0.Views.VehicleView;
using Drive_Smart_2._0.Views.VehicleView.Database;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Data.Sqlite;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

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
            RevenueDatabase.InitializeDatabase();
            RevenueDatabase.SeedSampleDataIfEmpty();
            LoadStats();
            LoadVehicleTable();
            LoadRevenueCharts();
            SidebarMenu.SetActivePage(ActivePage.Dashboard);
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

            // Monthly revenue — real figure from logged rental transactions
            // for the current calendar month (previously this was a rough
            // guess: avg(DailyRate) * rentedCount * 30, which didn't reflect
            // actual completed/active rentals).
            double revenue = RevenueDatabase.GetCurrentMonthRevenue();
            if (revenue > 0)
            {
                monthlyincometxt.Text = "Rs. " +
                    ((int)(revenue / 1000)).ToString() + "K";
            }
            else
            {
                monthlyincometxt.Text = "Rs. 0";
            }
        }

        private void LoadRevenueCharts()
        {
            // ---- Monthly trend (line chart, last 6 months) ----
            var monthly = RevenueDatabase.GetMonthlyRevenue(6);

            var trendSeries = new LineSeries<double>
            {
                Values = monthly.Select(m => m.Total).ToArray(),
                Name = "Revenue",
                Fill = new SolidColorPaint(new SKColor(0x21, 0x96, 0xF3, 60)),
                Stroke = new SolidColorPaint(new SKColor(0x15, 0x65, 0xC0)) { StrokeThickness = 3 },
                GeometryStroke = new SolidColorPaint(new SKColor(0x15, 0x65, 0xC0)) { StrokeThickness = 3 },
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometrySize = 8,
                LineSmoothness = 0.3
            };

            revenueTrendChart.Series = new ISeries[] { trendSeries };
            revenueTrendChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = monthly.Select(m => m.MonthLabel).ToArray(),
                    LabelsRotation = 0
                }
            };
            revenueTrendChart.YAxes = new Axis[]
            {
                new Axis { Labeler = value => "Rs. " + (value / 1000).ToString("N0") + "K" }
            };

            // ---- Category breakdown (bar chart, last 6 months) ----
            var byCategory = RevenueDatabase.GetRevenueByCategory(6);

            var categorySeries = new ColumnSeries<double>
            {
                Values = byCategory.Select(c => c.Total).ToArray(),
                Name = "Revenue",
                Fill = new SolidColorPaint(new SKColor(0x1E, 0x88, 0xE5)),
                MaxBarWidth = 45
            };

            revenueCategoryChart.Series = new ISeries[] { categorySeries };
            revenueCategoryChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = byCategory.Select(c => c.Category).ToArray(),
                    LabelsRotation = 0
                }
            };
            revenueCategoryChart.YAxes = new Axis[]
            {
                new Axis { Labeler = value => "Rs. " + (value / 1000).ToString("N0") + "K" }
            };
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