using Drive_Smart_2._0.Views.VehicleView.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class AdminVehicleView : Window
    {
        private string _selectedImagePath = null;

        public AdminVehicleView()
        {
            InitializeComponent();
            VehicleDatabase.InitializeDatabase();
            LoadVehicles();
        }

        // ── Pick Image ───────────────────────────────────────────────────────
        private void btnPickImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Vehicle Image",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;
                txtImageName.Text = Path.GetFileName(_selectedImagePath);
                imgPreview.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        // ── Save image file to project folder ────────────────────────────────
        private string SaveImageToProject(string sourcePath, string plateNumber)
        {
            if (string.IsNullOrEmpty(sourcePath)) return null;
            Directory.CreateDirectory(VehicleDatabase.ImagesFolder);
            string ext = Path.GetExtension(sourcePath);
            string fileName = $"{plateNumber.Replace(" ", "_")}{ext}";
            string destPath = Path.Combine(VehicleDatabase.ImagesFolder, fileName);
            File.Copy(sourcePath, destPath, overwrite: true);
            return Path.Combine("Views", "VehicleView", "Images", fileName);
        }

        // ── Load vehicles into DataGrid ───────────────────────────────────────
        private void LoadVehicles()
        {
            var list = new List<Vehicle>();
            using var conn = VehicleDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath FROM Vehicles";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Vehicle
                {
                    Id = reader.GetInt32(0),
                    Brand = reader.GetString(1),
                    Model = reader.GetString(2),
                    PlateNumber = reader.GetString(3),
                    Year = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    Color = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    DailyRate = reader.GetDouble(6),
                    Status = reader.GetString(7),
                    ImagePath = reader.IsDBNull(8) ? null : reader.GetString(8)
                });
            }
            dgVehicles.ItemsSource = list;
        }

        // ── Row selected → fill form ──────────────────────────────────────────
        private void dgVehicles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVehicles.SelectedItem is Vehicle v)
            {
                txtBrand.Text = v.Brand;
                txtModel.Text = v.Model;
                txtPlate.Text = v.PlateNumber;
                txtYear.Text = v.Year.ToString();
                txtColor.Text = v.Color;
                txtRate.Text = v.DailyRate.ToString();

                if (!string.IsNullOrEmpty(v.ImagePath))
                {
                    string projectRoot = Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
                    string fullPath = Path.Combine(projectRoot, v.ImagePath);
                    if (File.Exists(fullPath))
                    {
                        imgPreview.Source = new BitmapImage(new Uri(fullPath));
                        txtImageName.Text = Path.GetFileName(fullPath);
                    }
                }
                else
                {
                    imgPreview.Source = null;
                    txtImageName.Text = "No image";
                }
                _selectedImagePath = null;
            }
        }

        // ── ADD ──────────────────────────────────────────────────────────────
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            try
            {
                string imagePath = SaveImageToProject(_selectedImagePath, txtPlate.Text.Trim());
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Vehicles (Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath)
                    VALUES ($brand, $model, $plate, $year, $color, $rate, 'Available', $img)";
                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$model", txtModel.Text.Trim());
                cmd.Parameters.AddWithValue("$plate", txtPlate.Text.Trim());
                cmd.Parameters.AddWithValue("$year", int.Parse(txtYear.Text.Trim()));
                cmd.Parameters.AddWithValue("$color", txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$rate", double.Parse(txtRate.Text.Trim()));
                cmd.Parameters.AddWithValue("$img", imagePath ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Vehicle added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadVehicles();
            }
            catch (SqliteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("Plate number already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── MODIFY ───────────────────────────────────────────────────────────
        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehicles.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Select a vehicle first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidateForm()) return;
            try
            {
                string imagePath = _selectedImagePath != null
                    ? SaveImageToProject(_selectedImagePath, txtPlate.Text.Trim())
                    : selected.ImagePath;
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Vehicles
                    SET Brand=$brand, Model=$model, PlateNumber=$plate,
                        Year=$year, Color=$color, DailyRate=$rate, ImagePath=$img
                    WHERE Id=$id";
                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$model", txtModel.Text.Trim());
                cmd.Parameters.AddWithValue("$plate", txtPlate.Text.Trim());
                cmd.Parameters.AddWithValue("$year", int.Parse(txtYear.Text.Trim()));
                cmd.Parameters.AddWithValue("$color", txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$rate", double.Parse(txtRate.Text.Trim()));
                cmd.Parameters.AddWithValue("$img", imagePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$id", selected.Id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Vehicle updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── DUPLICATE ────────────────────────────────────────────────────────
        private void btnDuplicate_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehicles.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Select a vehicle first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Vehicles (Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath)
                    VALUES ($brand, $model, $plate, $year, $color, $rate, 'Available', $img)";
                cmd.Parameters.AddWithValue("$brand", selected.Brand);
                cmd.Parameters.AddWithValue("$model", selected.Model);
                cmd.Parameters.AddWithValue("$plate", selected.PlateNumber + "_COPY");
                cmd.Parameters.AddWithValue("$year", selected.Year);
                cmd.Parameters.AddWithValue("$color", selected.Color);
                cmd.Parameters.AddWithValue("$rate", selected.DailyRate);
                cmd.Parameters.AddWithValue("$img", selected.ImagePath ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Duplicated! Update the plate number.", "Duplicated", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── REMOVE ───────────────────────────────────────────────────────────
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehicles.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Select a vehicle first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Delete {selected.Brand} {selected.Model}?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Vehicles WHERE Id = $id";
                cmd.Parameters.AddWithValue("$id", selected.Id);
                cmd.ExecuteNonQuery();
                ClearForm();
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── TOGGLE STATUS ────────────────────────────────────────────────────
        private void btnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehicles.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Select a vehicle first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string newStatus = selected.Status == "Available" ? "Unavailable" : "Available";
            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Vehicles SET Status = $status WHERE Id = $id";
                cmd.Parameters.AddWithValue("$status", newStatus);
                cmd.Parameters.AddWithValue("$id", selected.Id);
                cmd.ExecuteNonQuery();
                MessageBox.Show($"Status changed to: {newStatus}", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── CLEAR ────────────────────────────────────────────────────────────
        private void btnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtBrand.Text = txtModel.Text = txtPlate.Text =
            txtYear.Text = txtColor.Text = txtRate.Text = "";
            imgPreview.Source = null;
            txtImageName.Text = "No image selected";
            _selectedImagePath = null;
            dgVehicles.SelectedItem = null;
        }

        // ── VALIDATION ───────────────────────────────────────────────────────
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtPlate.Text))
            {
                MessageBox.Show("Brand, Model and Plate are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(txtYear.Text, out _))
            {
                MessageBox.Show("Year must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!double.TryParse(txtRate.Text, out _))
            {
                MessageBox.Show("Daily Rate must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // ── NAV ──────────────────────────────────────────────────────────────
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new PublicVehicleView().Show();
            this.Close();
        }
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string PlateNumber { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public double DailyRate { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
    }
}