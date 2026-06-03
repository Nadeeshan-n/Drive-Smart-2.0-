using Drive_Smart_2._0.Views.VehicleView.Database;
using Microsoft.Data.Sqlite;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class AdminVehicleView : Window
    {
        public AdminVehicleView()
        {
            InitializeComponent();
            VehicleDatabase.InitializeDatabase();
            LoadVehicles();

            // ADD THIS - shows you the exact path being used
            MessageBox.Show("DB will be created at:\n" + VehicleDatabase.DbPath);

            VehicleDatabase.InitializeDatabase();
            LoadVehicles();

        }

        // ── Load all vehicles into the DataGrid ──────────────────────────────
        private void LoadVehicles()
        {
            var list = new List<Vehicle>();
            using var conn = VehicleDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, PlateNumber, Year, Color, DailyRate, Status FROM Vehicles";
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
            dgVehicles.ItemsSource = list;
        }

        // ── When a row is selected, fill the form fields ─────────────────────
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
            }
        }

        // ── ADD ──────────────────────────────────────────────────────────────
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Vehicles (Brand, Model, PlateNumber, Year, Color, DailyRate, Status)
                    VALUES ($brand, $model, $plate, $year, $color, $rate, 'Available')";
                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$model", txtModel.Text.Trim());
                cmd.Parameters.AddWithValue("$plate", txtPlate.Text.Trim());
                cmd.Parameters.AddWithValue("$year", int.Parse(txtYear.Text.Trim()));
                cmd.Parameters.AddWithValue("$color", txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$rate", double.Parse(txtRate.Text.Trim()));
                cmd.ExecuteNonQuery();

                MessageBox.Show("Vehicle added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadVehicles();
            }
            catch (SqliteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("A vehicle with that plate number already exists.", "Duplicate Plate", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Please select a vehicle to modify.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ValidateForm()) return;

            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE Vehicles
                    SET Brand=$brand, Model=$model, PlateNumber=$plate,
                        Year=$year, Color=$color, DailyRate=$rate
                    WHERE Id=$id";
                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$model", txtModel.Text.Trim());
                cmd.Parameters.AddWithValue("$plate", txtPlate.Text.Trim());
                cmd.Parameters.AddWithValue("$year", int.Parse(txtYear.Text.Trim()));
                cmd.Parameters.AddWithValue("$color", txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$rate", double.Parse(txtRate.Text.Trim()));
                cmd.Parameters.AddWithValue("$id", selected.Id);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Vehicle updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Please select a vehicle to duplicate.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                // Append _COPY to plate number to avoid UNIQUE conflict
                cmd.CommandText = @"
                    INSERT INTO Vehicles (Brand, Model, PlateNumber, Year, Color, DailyRate, Status)
                    VALUES ($brand, $model, $plate, $year, $color, $rate, 'Available')";
                cmd.Parameters.AddWithValue("$brand", selected.Brand);
                cmd.Parameters.AddWithValue("$model", selected.Model);
                cmd.Parameters.AddWithValue("$plate", selected.PlateNumber + "_COPY");
                cmd.Parameters.AddWithValue("$year", selected.Year);
                cmd.Parameters.AddWithValue("$color", selected.Color);
                cmd.Parameters.AddWithValue("$rate", selected.DailyRate);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Vehicle duplicated! Update the plate number.", "Duplicated", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("Please select a vehicle to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete {selected.Brand} {selected.Model} ({selected.PlateNumber})?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

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

        // ── CLEAR ────────────────────────────────────────────────────────────
        private void btnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtBrand.Text = "";
            txtModel.Text = "";
            txtPlate.Text = "";
            txtYear.Text = "";
            txtColor.Text = "";
            txtRate.Text = "";
            dgVehicles.SelectedItem = null;
        }

        // ── VALIDATION ───────────────────────────────────────────────────────
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtPlate.Text))
            {
                MessageBox.Show("Brand, Model and Plate Number are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        // ── NAV buttons ──────────────────────────────────────────────────────
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var view = new PublicVehicleView();
            view.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) { }
    }

    // ── Vehicle model class ──────────────────────────────────────────────────
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