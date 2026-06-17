using Drive_Smart_2._0.Views.VehicleView.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;


namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class PublicVehicleView : Page
    {
        private List<VehicleTile> _allVehicles = new();

        public PublicVehicleView()
        {
            InitializeComponent();
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            var list = new List<VehicleTile>();
            string projectRoot = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));

            using var conn = VehicleDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath FROM Vehicles";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string relPath = reader.IsDBNull(7) ? null : reader.GetString(7);
                string fullPath = relPath != null ? Path.Combine(projectRoot, relPath) : null;

                BitmapImage image = null;
                if (fullPath != null && File.Exists(fullPath))
                {
                    image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(fullPath);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                }

                list.Add(new VehicleTile
                {
                    Brand = reader.GetString(0),
                    Model = reader.GetString(1),
                    PlateNumber = reader.GetString(2),
                    Year = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    Color = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    DailyRate = reader.GetDouble(5),
                    Status = reader.GetString(6),
                    FullImagePath = image
                });
            }

            _allVehicles = list;
            icVehicles.ItemsSource = _allVehicles;
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string query = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(query))
            {
                icVehicles.ItemsSource = _allVehicles;
            }
            else
            {
                var filtered = _allVehicles.Where(v =>
                    v.BrandModel.ToLower().Contains(query) ||
                    v.PlateNumber.ToLower().Contains(query) ||
                    v.Color.ToLower().Contains(query) ||
                    v.Status.ToLower().Contains(query) ||
                    v.Year.ToString().Contains(query)
                ).ToList();

                icVehicles.ItemsSource = filtered;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //new AdminVehicleView().Show();
            //this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VVMainWindow vVMainWindow = new VVMainWindow();
            vVMainWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();

        }
    }

    public class VehicleTile
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string BrandModel => $"{Brand} {Model}";
        public string PlateNumber { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public double DailyRate { get; set; }
        public string Status { get; set; }
        public BitmapImage FullImagePath { get; set; }

        public SolidColorBrush StatusColor => Status == "Available"
            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(67, 160, 71))
            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 57, 53));
    }
}




//--------------------------
// DATABSE CONNECION START |
//--------------------------
/*
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

        MessageBox.Show("Database Connected Successfully!");
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
*/
//------------------------
// DATABSE CONNECION END |
//------------------------