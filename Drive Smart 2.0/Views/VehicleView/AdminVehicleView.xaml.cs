using Drive_Smart_2._0.Views.VehicleView.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class AdminVehicleView : Page
    {
        private string _selectedImagePath = null;

        // ════════════════════════════════════════════════════════════════════
        // SRI LANKA NUMBER PLATE SYSTEM
        //
        // OLD FORMAT 1 : 2 letters + 4 digits        AB-1234
        // OLD FORMAT 2 : 2-3 digits + 4 digits       61-1234  |  100-1234
        // NEW FORMAT   : 3 letters + 4 digits         CAB-1234
        //
        // Separator is optional — auto-inserted if missing (just testing)
        // ════════════════════════════════════════════════════════════════════

        private static readonly Dictionary<string, string> PlateCategoryMap =
            BuildPlateCategoryMap();

        private static Dictionary<string, string> BuildPlateCategoryMap()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string[] carFirstLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "R", "S", "T", "W", "X", "Y", "Z" };
            string[] allLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            // NEW FORMAT: 3 letters
            foreach (var first in carFirstLetters)
                foreach (var second in allLetters)
                    foreach (var third in allLetters)
                    {
                        string prefix = first + second + third;
                        if (first == "B" && "CDEFG".Contains(second))
                            map[prefix] = "Motorcycle/Bike";
                        else if (first == "Q")
                            map[prefix] = "Three-Wheeler";
                        else
                            map[prefix] = "Car";
                    }

            // OLD FORMAT: 2 letters
            foreach (var first in carFirstLetters)
                foreach (var second in allLetters)
                    map[first + second] = "Car";

            // NUMERIC SERIES
            for (int i = 50; i <= 59; i++) map[i.ToString()] = "Van";
            for (int i = 60; i <= 79; i++) map[i.ToString()] = "Lorry/Truck";
            for (int i = 80; i <= 99; i++) map[i.ToString()] = "Bus";
            for (int i = 100; i <= 199; i++) map[i.ToString()] = "Car";
            for (int i = 300; i <= 319; i++) map[i.ToString()] = "Three-Wheeler";

            return map;
        }

        // ── Known Brand → Valid Models ────────────────────────────────────
        private static readonly Dictionary<string, string[]> BrandModelMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Toyota"] = new[] { "Corolla", "Camry", "Prius", "HiAce", "Land Cruiser", "Axio", "Allion", "Premio", "Aqua", "Vitz", "Hilux", "Coaster", "KDH", "Townace", "Raize", "Rush" },
                ["Honda"] = new[] { "Civic", "Fit", "Vezel", "HR-V", "CR-V", "Accord", "Stream", "Freed", "Jazz", "Insight", "CR-Z", "WR-V", "CB125", "CB150", "CBR150", "CBR250", "XR150" },
                ["Suzuki"] = new[] { "Swift", "Alto", "Wagon R", "Vitara", "Every", "Carry", "Jimny", "Baleno", "Celerio", "Ertiga", "GSX125", "Gixxer", "Raider", "Burgman", "Address" },
                ["Nissan"] = new[] { "X-Trail", "Leaf", "March", "Tiida", "Note", "Navara", "Caravan", "Atlas", "Sunny", "AD Wagon", "Dayz", "Wingroad" },
                ["Mitsubishi"] = new[] { "Lancer", "Montero", "Outlander", "L200", "Rosa", "Canter", "Fuso", "Attrage", "Eclipse Cross", "ASX", "Delica" },
                ["Isuzu"] = new[] { "D-Max", "ELF", "Forward", "Giga", "NPR", "NQR", "NPS", "MU-X" },
                ["Tata"] = new[] { "Nano", "Indica", "Indigo", "Prima", "Ultra", "Xenon", "Ace", "407", "709" },
                ["Ashok Leyland"] = new[] { "Dost", "Partner", "Boss", "Captain", "Viking", "Comet", "Lynx", "Stallion" },
                ["Yamaha"] = new[] { "FZ", "FZS", "R15", "R1", "MT-15", "NMAX", "Fascino", "Ray-ZR", "Aerox", "Saluto", "SZ-RR" },
                ["Bajaj"] = new[] { "Pulsar 150", "Pulsar 180", "Pulsar 200", "CT100", "Platina", "Discover", "Avenger", "RE" },
                ["TVS"] = new[] { "Apache RTR 150", "Apache RTR 160", "Star City", "Sport", "XL 100", "King", "Wego", "Jupiter", "Ntorq" },
                ["Hero"] = new[] { "Splendor", "Passion", "Glamour", "Xtreme", "HF Deluxe", "Maestro", "Duet" },
                ["KTM"] = new[] { "Duke 200", "Duke 390", "RC 200", "RC 390", "Adventure 390" },
                ["Volvo"] = new[] { "B7R", "B9R", "FM", "FH", "FMX", "B8R", "9400" },
                ["Leyland"] = new[] { "Comet", "Hippo", "Titan", "Lynx", "Viking", "Dost" },
                ["DFSK"] = new[] { "Glory 580", "Glory 500", "K01", "K02", "V21", "V22" },
                ["Force"] = new[] { "Traveller", "Trax", "Gurkha", "Tempo" },
            };

        // ════════════════════════════════════════════════════════════════════
        public AdminVehicleView()
        {
            InitializeComponent();
            VehicleDatabase.InitializeDatabase();
            LoadVehicles();
            UpdatePlateHint();
            LoadMaintenanceBadge();
        }



        // ── Maintanance Batch ──────────────────────────────────────────────

        private void LoadMaintenanceBadge()
        {
            int count = MaintenanceDatabase.GetUrgentCount();
            maintenanceBadge.Visibility = count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            maintenanceBadgeCount.Text = count.ToString();
        }



        // ── Category changed ──────────────────────────────────────────────
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePlateHint();
            if (!string.IsNullOrWhiteSpace(txtPlate?.Text))
                RunPlateValidation(txtPlate.Text.Trim());
        }

        private void UpdatePlateHint()
        {
            if (txtPlateHint == null || cmbCategory?.SelectedItem is not ComboBoxItem item) return;
            txtPlateHint.Text = item.Content.ToString() switch
            {
                "Car" => "Old: AB-1234  |  New: CAB-1234  |  Numeric: 100-1234",
                "Motorcycle/Bike" => "New 3-letter: BCB-1234  BDB-1234  BEB-1234",
                "Three-Wheeler" => "3-letter: QAB-1234  |  Numeric: 300-1234",
                "Van" => "Numeric: 50-1234  to  59-1234",
                "Lorry/Truck" => "Numeric: 60-1234  to  79-1234",
                "Bus" => "Numeric: 80-1234  to  99-1234",
                _ => "Old: AB-1234  |  New: CAB-1234"
            };
        }

        // ── Real-time plate validation ────────────────────────────────────
        private void txtPlate_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunPlateValidation(txtPlate.Text.Trim());
        }

        private void RunPlateValidation(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
            {
                SetIndicator(null);
                HideValidation();
                return;
            }

            var (valid, message, detectedCategory) = ValidateSriLankanPlate(plate);

            if (valid)
            {
                SetIndicator(true);
                if (!string.IsNullOrEmpty(message))
                    ShowValidation(message); // info message e.g. category auto-updated
                else
                    HideValidation();

                if (!string.IsNullOrEmpty(detectedCategory))
                    SetCategory(detectedCategory);
            }
            else
            {
                SetIndicator(false);
                ShowValidation(message);
            }
        }

        // ── Core Sri Lankan plate validation ─────────────────────────────
        private (bool valid, string message, string detectedCategory) ValidateSriLankanPlate(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return (false, "Plate number is required.", null);

            // Normalize
            string plate = raw.ToUpper().Trim()
                .Replace(" ", "-")
                .Replace("–", "-")
                .Replace("—", "-");

            while (plate.Contains("--"))
                plate = plate.Replace("--", "-");

            // Auto-insert dash if missing
            if (!plate.Contains("-"))
            {
                if (Regex.IsMatch(plate, @"^[A-Z]{3}[0-9]{4}$"))
                    plate = plate[..3] + "-" + plate[3..];
                else if (Regex.IsMatch(plate, @"^[A-Z]{2}[0-9]{4}$"))
                    plate = plate[..2] + "-" + plate[2..];
                else if (Regex.IsMatch(plate, @"^[0-9]{3}[0-9]{4}$"))
                    plate = plate[..3] + "-" + plate[3..];
                else if (Regex.IsMatch(plate, @"^[0-9]{2}[0-9]{4}$"))
                    plate = plate[..2] + "-" + plate[2..];
                else
                    return (false,
                        "Cannot determine plate format.\n" +
                        "• Old car:    AB-1234\n" +
                        "• New car:    CAB-1234\n" +
                        "• Lorry/Bus:  61-1234\n" +
                        "• Old numeric: 100-1234",
                        null);
            }

            string[] parts = plate.Split('-');
            if (parts.Length != 2)
                return (false, "Use exactly one separator dash: AB-1234 or CAB-1234", null);

            string prefix = parts[0];
            string digits = parts[1];

            // Digits must be exactly 4
            if (!Regex.IsMatch(digits, @"^[0-9]{4}$"))
                return (false,
                    $"The number part must be exactly 4 digits.\nYou entered: '{digits}'\nExample: AB-1234",
                    null);

            bool isLetterPrefix = Regex.IsMatch(prefix, @"^[A-Z]{1,3}$");
            bool isNumericPrefix = Regex.IsMatch(prefix, @"^[0-9]{2,3}$");

            if (!isLetterPrefix && !isNumericPrefix)
                return (false,
                    $"Prefix '{prefix}' is not valid.\n" +
                    "Must be:\n" +
                    "• 2 letters (AB) — old format\n" +
                    "• 3 letters (CAB) — new format\n" +
                    "• 2-3 digits (61, 100) — numeric series",
                    null);

            // Letter prefix rules
            if (isLetterPrefix)
            {
                if (prefix.Length == 1)
                    return (false,
                        "Single-letter prefix is not valid in Sri Lanka.\n" +
                        "Use AB-1234 (old) or CAB-1234 (new).",
                        null);

                // 2 letters = old format, 3 letters = new format — both valid
            }

            // Numeric prefix rules
            if (isNumericPrefix && int.TryParse(prefix, out int num))
            {
                if (num < 50)
                    return (false,
                        $"Numeric prefix '{prefix}' is too low.\n" +
                        "Sri Lanka numeric plates start from 50.\n" +
                        "• Vans: 50-59\n• Lorries: 60-79\n• Buses: 80-99\n• Old cars: 100-199\n• Three-wheelers: 300-319",
                        null);

                if (num >= 200 && num < 300)
                    return (false,
                        $"Numeric prefix '{prefix}' (200-299) is not an assigned series in Sri Lanka.",
                        null);

                if (num > 319)
                    return (false,
                        $"Numeric prefix '{prefix}' is out of range.\n" +
                        "Valid ranges: 50-99, 100-199, 300-319",
                        null);
            }

            // Detect category
            string detectedCategory = PlateCategoryMap.TryGetValue(prefix, out string cat) ? cat : null;
            string selectedCategory = GetSelectedCategory();

            if (detectedCategory != null && detectedCategory != selectedCategory)
                return (true,
                    $"ℹ Prefix '{prefix}' is a {detectedCategory} series. Category auto-updated.",
                    detectedCategory);

            return (true, null, detectedCategory);
        }

        // ── Brand + Model cross validation ───────────────────────────────
        private void txtBrand_TextChanged(object sender, TextChangedEventArgs e)
            => CrossValidateBrandModel();

        private void txtModel_TextChanged(object sender, TextChangedEventArgs e)
            => CrossValidateBrandModel();

        private void CrossValidateBrandModel()
        {
            string brand = txtBrand?.Text.Trim() ?? "";
            string model = txtModel?.Text.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model)) return;

            if (BrandModelMap.TryGetValue(brand, out string[] models))
            {
                bool found = false;
                foreach (var m in models)
                    if (m.Equals(model, StringComparison.OrdinalIgnoreCase))
                    { found = true; break; }

                if (!found)
                {
                    ShowValidation($"'{model}' is not a known model for {brand}.\nKnown models: {string.Join(", ", models)}");
                    return;
                }
            }

            HideValidation();
        }

        // ── UI helpers ────────────────────────────────────────────────────
        private void SetIndicator(bool? valid)
        {
            if (txtPlateIndicator == null) return;
            txtPlateIndicator.Foreground = valid switch
            {
                true => new SolidColorBrush(Colors.Green),
                false => new SolidColorBrush(Colors.Red),
                null => new SolidColorBrush(Colors.LightGray)
            };
        }

        private void ShowValidation(string message)
        {
            txtValidationMsg.Text = message;
            borderValidation.Visibility = Visibility.Visible;
        }

        private void HideValidation()
        {
            borderValidation.Visibility = Visibility.Collapsed;
            txtValidationMsg.Text = "";
        }

        private string GetSelectedCategory()
        {
            if (cmbCategory?.SelectedItem is ComboBoxItem item)
                return item.Content.ToString();
            return "Car";
        }

        private void SetCategory(string category)
        {
            foreach (ComboBoxItem item in cmbCategory.Items)
                if (item.Content.ToString() == category)
                { cmbCategory.SelectedItem = item; return; }
        }

        // ── Pick Image ────────────────────────────────────────────────────
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

        private string SaveImageToProject(string sourcePath, string plateNumber)
        {
            if (string.IsNullOrEmpty(sourcePath)) return null;
            Directory.CreateDirectory(VehicleDatabase.ImagesFolder);
            string ext = Path.GetExtension(sourcePath);
            string fileName = $"{plateNumber.Replace(" ", "_").Replace("-", "_")}{ext}";
            string destPath = Path.Combine(VehicleDatabase.ImagesFolder, fileName);
            File.Copy(sourcePath, destPath, overwrite: true);
            return Path.Combine("Views", "VehicleView", "Images", fileName);
        }

        // ── Load vehicles ─────────────────────────────────────────────────
        private void LoadVehicles()
        {
            var list = new List<Vehicle>();
            using var conn = VehicleDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath, Category FROM Vehicles";
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
                    ImagePath = reader.IsDBNull(8) ? null : reader.GetString(8),
                    Category = reader.IsDBNull(9) ? "Car" : reader.GetString(9)
                });
            }
            dgVehicles.ItemsSource = list;
        }

        // ── Row selected → fill form ──────────────────────────────────────
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
                SetCategory(v.Category);

                if (!string.IsNullOrEmpty(v.ImagePath))
                {
                    string root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
                    string fullPath = Path.Combine(root, v.ImagePath);
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
                HideValidation();
            }
        }

        // ── ADD ───────────────────────────────────────────────────────────
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
                    INSERT INTO Vehicles (Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath, Category)
                    VALUES ($brand,$model,$plate,$year,$color,$rate,'Available',$img,$cat)";
                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$model", txtModel.Text.Trim());
                cmd.Parameters.AddWithValue("$plate", txtPlate.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("$year", int.Parse(txtYear.Text.Trim()));
                cmd.Parameters.AddWithValue("$color", txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$rate", double.Parse(txtRate.Text.Trim()));
                cmd.Parameters.AddWithValue("$img", imagePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$cat", GetSelectedCategory());
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

        // ── MODIFY ────────────────────────────────────────────────────────
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
                        Year=$year, Color=$color, DailyRate=$rate,
                        ImagePath=$img, Category=$cat
                    WHERE Id=$id";
                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$model", txtModel.Text.Trim());
                cmd.Parameters.AddWithValue("$plate", txtPlate.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("$year", int.Parse(txtYear.Text.Trim()));
                cmd.Parameters.AddWithValue("$color", txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$rate", double.Parse(txtRate.Text.Trim()));
                cmd.Parameters.AddWithValue("$img", imagePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$cat", GetSelectedCategory());
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

        // ── DUPLICATE ─────────────────────────────────────────────────────
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
                    INSERT INTO Vehicles (Brand, Model, PlateNumber, Year, Color, DailyRate, Status, ImagePath, Category)
                    VALUES ($brand,$model,$plate,$year,$color,$rate,'Available',$img,$cat)";
                cmd.Parameters.AddWithValue("$brand", selected.Brand);
                cmd.Parameters.AddWithValue("$model", selected.Model);
                cmd.Parameters.AddWithValue("$plate", selected.PlateNumber + "_COPY");
                cmd.Parameters.AddWithValue("$year", selected.Year);
                cmd.Parameters.AddWithValue("$color", selected.Color);
                cmd.Parameters.AddWithValue("$rate", selected.DailyRate);
                cmd.Parameters.AddWithValue("$img", selected.ImagePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$cat", selected.Category);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Duplicated! Update the plate number.", "Duplicated", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVehicles();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── REMOVE ────────────────────────────────────────────────────────
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (dgVehicles.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Select a vehicle first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Delete {selected.Brand} {selected.Model} ({selected.PlateNumber})?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                using var conn = VehicleDatabase.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Vehicles WHERE Id=$id";
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

        // ── TOGGLE STATUS ─────────────────────────────────────────────────
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
                cmd.CommandText = "UPDATE Vehicles SET Status=$status WHERE Id=$id";
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

        // ── CLEAR ─────────────────────────────────────────────────────────
        private void btnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtBrand.Text = txtModel.Text = txtPlate.Text =
            txtYear.Text = txtColor.Text = txtRate.Text = "";
            imgPreview.Source = null;
            txtImageName.Text = "No image selected";
            _selectedImagePath = null;
            cmbCategory.SelectedIndex = 0;
            dgVehicles.SelectedItem = null;
            SetIndicator(null);
            HideValidation();
        }

        // ── FORM VALIDATION on submit ─────────────────────────────────────
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtPlate.Text))
            {
                MessageBox.Show("Brand, Model and Plate are required.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var (plateValid, plateMsg, _) = ValidateSriLankanPlate(txtPlate.Text.Trim());
            if (!plateValid)
            {
                MessageBox.Show($"Invalid plate number:\n{plateMsg}", "Plate Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtYear.Text, out int year) || year < 1940 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show($"Enter a valid year between 1940 and {DateTime.Now.Year + 1}.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!double.TryParse(txtRate.Text, out double rate) || rate <= 0)
            {
                MessageBox.Show("Daily Rate must be a positive number.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // ── NAV ───────────────────────────────────────────────────────────
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //PublicVehicleView publicVehicleView = new PublicVehicleView();

            NavigationService.Navigate(new PublicVehicleView());


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminHelpView());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MaintenanceView());
        }
    }

    // ── Vehicle model class ───────────────────────────────────────────────
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
        public string Category { get; set; }
        public string DailyRateDisplay => $"LKR {DailyRate:N2}";
    }
}