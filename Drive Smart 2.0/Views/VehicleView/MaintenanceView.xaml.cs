using Drive_Smart_2._0.Views.Auth.Helpers;
using Drive_Smart_2._0.Views.VehicleView.Database;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class MaintenanceView : Window
    {
        private int? _editingId = null;

        public MaintenanceView()
        {

            InitializeComponent();
            MaintenanceDatabase.InitializeDatabase();
            LoadVehiclesIntoCombo();
            dpScheduled.SelectedDate = DateTime.Today;
            LoadRecords();
            SidebarMenu.SetActivePage(ActivePage.VehicleMaintenance);

        }

        // ── Load vehicles from vehicles.db into combo ─────────────────────
        private void LoadVehiclesIntoCombo()
        {
            cmbVehicle.Items.Clear();
            using var conn = VehicleDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Brand, Model, PlateNumber FROM Vehicles ORDER BY Brand";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cmbVehicle.Items.Add(new VehicleComboItem
                {
                    Id = reader.GetInt32(0),
                    Label = $"{reader.GetString(1)} {reader.GetString(2)} — {reader.GetString(3)}",
                    Plate = reader.GetString(3),
                    Name = $"{reader.GetString(1)} {reader.GetString(2)}"
                });
            }
            cmbVehicle.DisplayMemberPath = "Label";
        }

        private void cmbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtCustomType == null || cmbType?.SelectedItem == null) return;
            bool isCustom = cmbType.SelectedItem is ComboBoxItem ci && ci.Content.ToString() == "Custom";
            txtCustomType.IsEnabled = isCustom;
            txtCustomType.Background = isCustom
                ? System.Windows.Media.Brushes.White
                : System.Windows.Media.Brushes.WhiteSmoke;
        }

        // ── Load records into DataGrid ────────────────────────────────────
        private void LoadRecords(string filter = "All Records")
        {
            if (dgMaintenance == null) return;
            var list = new List<MaintenanceRecord>();
            using var conn = MaintenanceDatabase.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            string where = filter switch
            {
                "Overdue" => "WHERE Status != 'Completed' AND Status != 'Cancelled' AND date(ScheduledDate) < date('now')",
                "Due Soon" => "WHERE Status != 'Completed' AND Status != 'Cancelled' AND date(ScheduledDate) BETWEEN date('now') AND date('now','+7 days')",
                "Scheduled" => "WHERE Status = 'Scheduled'",
                "In Progress" => "WHERE Status = 'In Progress'",
                "Completed" => "WHERE Status = 'Completed'",
                _ => ""
            };

            cmd.CommandText = $@"
                SELECT Id, VehicleId, VehiclePlate, VehicleName, MaintenanceType, CustomType,
                       Status, ScheduledDate, CompletedDate, CurrentMileage, NextMileage,
                       IntervalDays, IntervalMileage, Cost, Notes, CreatedAt, UpdatedAt, CreatedBy
                FROM MaintenanceRecords {where}
                ORDER BY
                    CASE Status
                        WHEN 'In Progress' THEN 1
                        WHEN 'Scheduled'   THEN 2
                        WHEN 'Completed'   THEN 3
                        ELSE 4
                    END,
                    date(ScheduledDate) ASC";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var rec = new MaintenanceRecord
                {
                    Id = reader.GetInt32(0),
                    VehicleId = reader.GetInt32(1),
                    VehiclePlate = reader.GetString(2),
                    VehicleName = reader.GetString(3),
                    MaintenanceType = reader.GetString(4),
                    CustomType = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Status = reader.GetString(6),
                    ScheduledDate = reader.GetString(7),
                    CompletedDate = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CurrentMileage = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                    NextMileage = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                    IntervalDays = reader.GetInt32(11),
                    IntervalMileage = reader.GetInt32(12),
                    Cost = reader.IsDBNull(13) ? 0 : reader.GetDouble(13),
                    Notes = reader.IsDBNull(14) ? "" : reader.GetString(14),
                    CreatedAt = reader.GetString(15),
                    UpdatedAt = reader.GetString(16),
                    CreatedBy = reader.GetString(17)
                };
                list.Add(rec);
            }

            dgMaintenance.ItemsSource = list;
            UpdateStats(list);
        }

        // ── Update stat cards + badge ─────────────────────────────────────
        private void UpdateStats(List<MaintenanceRecord> list)
        {
            if (statOverdue == null || statDueSoon == null) return;
            int overdue = 0, dueSoon = 0, completed = 0;
            var today = DateTime.Today;

            foreach (var r in list)
            {
                if (r.Status == "Completed") { completed++; continue; }
                if (r.Status == "Cancelled") continue;
                if (DateTime.TryParse(r.ScheduledDate, out DateTime sd))
                {
                    if (sd.Date < today) overdue++;
                    else if ((sd.Date - today).TotalDays <= 7) dueSoon++;
                }
            }

            statOverdue.Text = overdue.ToString();
            statDueSoon.Text = dueSoon.ToString();
            statCompleted.Text = completed.ToString();
            statTotal.Text = list.Count.ToString();

            // Header summary
            txtOverdueCount.Text = overdue > 0 ? $"🔴 {overdue} Overdue" : "";
            txtDueSoonCount.Text = dueSoon > 0 ? $"🟡 {dueSoon} Due This Week" : "";

            // Sidebar badge
            /*int urgent = overdue + dueSoon;
            if (urgent > 0)
            {
                badgeBorder.Visibility = Visibility.Visible;
                badgeCount.Text = urgent.ToString();
            }
            else
            {
                badgeBorder.Visibility = Visibility.Collapsed;
            }*/
        }

        // ── Row selected → fill form ──────────────────────────────────────
        private void dgMaintenance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMaintenance.SelectedItem is not MaintenanceRecord r) return;
            _editingId = r.Id;

            // Set vehicle combo
            foreach (VehicleComboItem item in cmbVehicle.Items)
                if (item.Id == r.VehicleId) { cmbVehicle.SelectedItem = item; break; }

            // Set type combo
            foreach (ComboBoxItem item in cmbType.Items)
                if (item.Content.ToString() == r.MaintenanceType) { cmbType.SelectedItem = item; break; }

            txtCustomType.Text = r.CustomType ?? "";

            foreach (ComboBoxItem item in cmbStatus.Items)
                if (item.Content.ToString() == r.Status) { cmbStatus.SelectedItem = item; break; }

            dpScheduled.SelectedDate = DateTime.TryParse(r.ScheduledDate, out var sd) ? sd : DateTime.Today;
            dpCompleted.SelectedDate = DateTime.TryParse(r.CompletedDate, out var cd) ? cd : (DateTime?)null;

            txtMileage.Text = r.CurrentMileage.ToString();
            txtIntervalDays.Text = r.IntervalDays.ToString();
            txtIntervalMileage.Text = r.IntervalMileage.ToString();
            txtCost.Text = r.Cost > 0 ? r.Cost.ToString() : "";
            txtNotes.Text = r.Notes;
        }

        // ── SAVE (Add or Update) ──────────────────────────────────────────
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var vehicle = (VehicleComboItem)cmbVehicle.SelectedItem;
            string type = ((ComboBoxItem)cmbType.SelectedItem).Content.ToString();
            string custom = type == "Custom" ? txtCustomType.Text.Trim() : null;
            string status = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            int intervalDays = int.TryParse(txtIntervalDays.Text, out int id) ? id : 90;
            int intervalMileage = int.TryParse(txtIntervalMileage.Text, out int im) ? im : 5000;
            int mileage = int.TryParse(txtMileage.Text, out int ml) ? ml : 0;
            double cost = double.TryParse(txtCost.Text, out double c) ? c : 0;

            string scheduled = dpScheduled.SelectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
            string completed = dpCompleted.SelectedDate?.ToString("yyyy-MM-dd");
            int nextMileage = mileage + intervalMileage;

            // Calculate next due date from scheduled + interval
            string nextDue = dpScheduled.SelectedDate.HasValue
                ? dpScheduled.SelectedDate.Value.AddDays(intervalDays).ToString("yyyy-MM-dd")
                : DateTime.Today.AddDays(intervalDays).ToString("yyyy-MM-dd");

            using var conn = MaintenanceDatabase.GetConnection();
            conn.Open();

            if (_editingId == null)
            {
                // INSERT
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO MaintenanceRecords
                        (VehicleId, VehiclePlate, VehicleName, MaintenanceType, CustomType,
                         Status, ScheduledDate, CompletedDate, CurrentMileage, NextMileage,
                         IntervalDays, IntervalMileage, Cost, Notes, CreatedAt, UpdatedAt)
                    VALUES
                        ($vid,$plate,$name,$type,$custom,
                         $status,$sched,$comp,$ml,$nml,
                         $idays,$iml,$cost,$notes,$now,$now)";
                SetMaintenanceParams(cmd, vehicle, type, custom, status, scheduled,
                    completed, mileage, nextMileage, intervalDays, intervalMileage, cost,
                    txtNotes.Text.Trim(), now);
                cmd.ExecuteNonQuery();

                // Get new id
                var idCmd = conn.CreateCommand();
                idCmd.CommandText = "SELECT last_insert_rowid()";
                long newId = (long)idCmd.ExecuteScalar();

                LogAudit(conn, (int)newId, vehicle.Plate, type, "Record Created", null, "New record", now);
                UpsertReminder(conn, vehicle, type, scheduled, mileage, intervalDays, intervalMileage, now);
            }
            else
            {
                // Get old record for audit
                var old = GetRecord(conn, _editingId.Value);

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE MaintenanceRecords SET
                        VehicleId=$vid, VehiclePlate=$plate, VehicleName=$name,
                        MaintenanceType=$type, CustomType=$custom, Status=$status,
                        ScheduledDate=$sched, CompletedDate=$comp, CurrentMileage=$ml,
                        NextMileage=$nml, IntervalDays=$idays, IntervalMileage=$iml,
                        Cost=$cost, Notes=$notes, UpdatedAt=$now
                    WHERE Id=$id";
                SetMaintenanceParams(cmd, vehicle, type, custom, status, scheduled,
                    completed, mileage, nextMileage, intervalDays, intervalMileage, cost,
                    txtNotes.Text.Trim(), now);
                cmd.Parameters.AddWithValue("$id", _editingId.Value);
                cmd.ExecuteNonQuery();

                // Audit each changed field
                AuditChanges(conn, _editingId.Value, vehicle.Plate, type, old,
                    status, scheduled, completed, mileage, nextMileage,
                    intervalDays, intervalMileage, cost, txtNotes.Text.Trim(), now);

                UpsertReminder(conn, vehicle, type, scheduled, mileage, intervalDays, intervalMileage, now);
            }

            MessageBox.Show("Maintenance record saved!", "Saved",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearForm();
            LoadRecords(((ComboBoxItem)cmbFilter.SelectedItem).Content.ToString());
        }

        // ── Mark as Completed ─────────────────────────────────────────────
        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaintenance.SelectedItem is not MaintenanceRecord r)
            {
                MessageBox.Show("Select a record first.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            using var conn = MaintenanceDatabase.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE MaintenanceRecords
                SET Status='Completed', CompletedDate=$today, UpdatedAt=$now
                WHERE Id=$id";
            cmd.Parameters.AddWithValue("$today", today);
            cmd.Parameters.AddWithValue("$now", now);
            cmd.Parameters.AddWithValue("$id", r.Id);
            cmd.ExecuteNonQuery();

            LogAudit(conn, r.Id, r.VehiclePlate, r.MaintenanceType,
                "Status", r.Status, "Completed", now);

            // Auto-schedule next maintenance
            AutoScheduleNext(conn, r, now);

            MessageBox.Show($"Marked as completed!\nNext service auto-scheduled for {DateTime.Today.AddDays(r.IntervalDays):dd MMM yyyy}.",
                "Completed", MessageBoxButton.OK, MessageBoxImage.Information);

            ClearForm();
            LoadRecords(((ComboBoxItem)cmbFilter.SelectedItem).Content.ToString());
        }

        // ── Auto-schedule next maintenance ───────────────────────────────
        private void AutoScheduleNext(SqliteConnection conn, MaintenanceRecord r, string now)
        {
            string nextDate = DateTime.Today.AddDays(r.IntervalDays).ToString("yyyy-MM-dd");
            int nextMileage = r.NextMileage + r.IntervalMileage;

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO MaintenanceRecords
                    (VehicleId, VehiclePlate, VehicleName, MaintenanceType, CustomType,
                     Status, ScheduledDate, CurrentMileage, NextMileage,
                     IntervalDays, IntervalMileage, Notes, CreatedAt, UpdatedAt)
                VALUES
                    ($vid,$plate,$name,$type,$custom,
                     'Scheduled',$sched,$ml,$nml,
                     $idays,$iml,$notes,$now,$now)";
            cmd.Parameters.AddWithValue("$vid", r.VehicleId);
            cmd.Parameters.AddWithValue("$plate", r.VehiclePlate);
            cmd.Parameters.AddWithValue("$name", r.VehicleName);
            cmd.Parameters.AddWithValue("$type", r.MaintenanceType);
            cmd.Parameters.AddWithValue("$custom", r.CustomType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$sched", nextDate);
            cmd.Parameters.AddWithValue("$ml", r.NextMileage);
            cmd.Parameters.AddWithValue("$nml", nextMileage);
            cmd.Parameters.AddWithValue("$idays", r.IntervalDays);
            cmd.Parameters.AddWithValue("$iml", r.IntervalMileage);
            cmd.Parameters.AddWithValue("$notes", $"Auto-scheduled from record #{r.Id}");
            cmd.Parameters.AddWithValue("$now", now);
            cmd.ExecuteNonQuery();
        }

        // ── DELETE ────────────────────────────────────────────────────────
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaintenance.SelectedItem is not MaintenanceRecord r)
            {
                MessageBox.Show("Select a record first.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Delete maintenance record #{r.Id} for {r.VehicleName}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            using var conn = MaintenanceDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM MaintenanceRecords WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id", r.Id);
            cmd.ExecuteNonQuery();

            LogAudit(conn, r.Id, r.VehiclePlate, r.MaintenanceType,
                "Record Deleted", "Existed", "Deleted",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            ClearForm();
            LoadRecords(((ComboBoxItem)cmbFilter.SelectedItem).Content.ToString());
        }

        // ── Audit Log popup ───────────────────────────────────────────────
        private void btnAuditLog_Click(object sender, RoutedEventArgs e)
        {
            var log = new AuditLogWindow();
            log.ShowDialog();
        }

        // ── Filter changed ────────────────────────────────────────────────
        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMaintenance == null) return;
            if (cmbFilter.SelectedItem is ComboBoxItem item)
                LoadRecords(item.Content.ToString());
        }

        // ── Helper: set parameters ────────────────────────────────────────
        private static void SetMaintenanceParams(
            SqliteCommand cmd, VehicleComboItem vehicle, string type, string custom,
            string status, string scheduled, string completed, int mileage,
            int nextMileage, int intervalDays, int intervalMileage,
            double cost, string notes, string now)
        {
            cmd.Parameters.AddWithValue("$vid", vehicle.Id);
            cmd.Parameters.AddWithValue("$plate", vehicle.Plate);
            cmd.Parameters.AddWithValue("$name", vehicle.Name);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$custom", custom ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$status", status);
            cmd.Parameters.AddWithValue("$sched", scheduled);
            cmd.Parameters.AddWithValue("$comp", completed ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$ml", mileage);
            cmd.Parameters.AddWithValue("$nml", nextMileage);
            cmd.Parameters.AddWithValue("$idays", intervalDays);
            cmd.Parameters.AddWithValue("$iml", intervalMileage);
            cmd.Parameters.AddWithValue("$cost", cost);
            cmd.Parameters.AddWithValue("$notes", notes);
            cmd.Parameters.AddWithValue("$now", now);
        }

        // ── Helper: upsert reminder ───────────────────────────────────────
        private static void UpsertReminder(SqliteConnection conn,
            VehicleComboItem vehicle, string type, string scheduled,
            int mileage, int intervalDays, int intervalMileage, string now)
        {
            string nextDue = DateTime.TryParse(scheduled, out var sd)
                ? sd.AddDays(intervalDays).ToString("yyyy-MM-dd")
                : DateTime.Today.AddDays(intervalDays).ToString("yyyy-MM-dd");

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO MaintenanceReminders
                    (VehicleId, VehiclePlate, MaintenanceType, IntervalDays, IntervalMileage,
                     LastServiceDate, LastServiceMileage, NextDueDate, NextDueMileage)
                VALUES ($vid,$plate,$type,$idays,$iml,$last,$lml,$next,$nml)
                ON CONFLICT(VehicleId, MaintenanceType) DO UPDATE SET
                    IntervalDays=excluded.IntervalDays,
                    IntervalMileage=excluded.IntervalMileage,
                    LastServiceDate=excluded.LastServiceDate,
                    LastServiceMileage=excluded.LastServiceMileage,
                    NextDueDate=excluded.NextDueDate,
                    NextDueMileage=excluded.NextDueMileage";
            cmd.Parameters.AddWithValue("$vid", vehicle.Id);
            cmd.Parameters.AddWithValue("$plate", vehicle.Plate);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$idays", intervalDays);
            cmd.Parameters.AddWithValue("$iml", intervalMileage);
            cmd.Parameters.AddWithValue("$last", scheduled);
            cmd.Parameters.AddWithValue("$lml", mileage);
            cmd.Parameters.AddWithValue("$next", nextDue);
            cmd.Parameters.AddWithValue("$nml", mileage + intervalMileage);
            cmd.ExecuteNonQuery();
        }

        // ── Helper: get old record for audit ──────────────────────────────
        private static MaintenanceRecord GetRecord(SqliteConnection conn, int id)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Status, ScheduledDate, CompletedDate, CurrentMileage, Cost, Notes FROM MaintenanceRecords WHERE Id=$id";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return new MaintenanceRecord
                {
                    Status = r.GetString(0),
                    ScheduledDate = r.GetString(1),
                    CompletedDate = r.IsDBNull(2) ? null : r.GetString(2),
                    CurrentMileage = r.IsDBNull(3) ? 0 : r.GetInt32(3),
                    Cost = r.IsDBNull(4) ? 0 : r.GetDouble(4),
                    Notes = r.IsDBNull(5) ? "" : r.GetString(5)
                };
            return null;
        }

        // ── Helper: audit field changes ───────────────────────────────────
        private static void AuditChanges(SqliteConnection conn, int id,
            string plate, string type, MaintenanceRecord old,
            string newStatus, string newSched, string newComp,
            int newMileage, int newNextMileage,
            int newIntDays, int newIntMileage,
            double newCost, string newNotes, string now)
        {
            if (old == null) return;
            void Log(string field, string oldVal, string newVal)
            {
                if (oldVal == newVal) return;
                LogAudit(conn, id, plate, type, field, oldVal, newVal, now);
            }
            Log("Status", old.Status, newStatus);
            Log("ScheduledDate", old.ScheduledDate, newSched);
            Log("CompletedDate", old.CompletedDate ?? "—", newComp ?? "—");
            Log("Mileage", old.CurrentMileage.ToString(), newMileage.ToString());
            Log("Cost", old.Cost.ToString(), newCost.ToString());
            Log("Notes", old.Notes, newNotes);
        }

        // ── Helper: write audit log entry ─────────────────────────────────
        private static void LogAudit(SqliteConnection conn, int recordId,
            string plate, string type, string field,
            string oldVal, string newVal, string now)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO MaintenanceAuditLog
                    (RecordId, VehiclePlate, MaintenanceType, FieldChanged, OldValue, NewValue, ChangedAt)
                VALUES ($rid,$plate,$type,$field,$old,$new,$now)";
            cmd.Parameters.AddWithValue("$rid", recordId);
            cmd.Parameters.AddWithValue("$plate", plate);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$field", field);
            cmd.Parameters.AddWithValue("$old", oldVal ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$new", newVal ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$now", now);
            cmd.ExecuteNonQuery();
        }

        // ── CLEAR ─────────────────────────────────────────────────────────
        private void btnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _editingId = null;
            cmbVehicle.SelectedItem = null;
            cmbType.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            txtCustomType.Text = "";
            txtMileage.Text = "";
            txtIntervalDays.Text = "90";
            txtIntervalMileage.Text = "5000";
            txtCost.Text = "";
            txtNotes.Text = "";
            dpScheduled.SelectedDate = DateTime.Today;
            dpCompleted.SelectedDate = null;
            dgMaintenance.SelectedItem = null;
        }

        // ── VALIDATION ────────────────────────────────────────────────────
        private bool ValidateForm()
        {
            if (cmbVehicle.SelectedItem == null)
            {
                MessageBox.Show("Please select a vehicle.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (cmbType.SelectedItem == null)
            {
                MessageBox.Show("Please select a maintenance type.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (((ComboBoxItem)cmbType.SelectedItem).Content.ToString() == "Custom"
                && string.IsNullOrWhiteSpace(txtCustomType.Text))
            {
                MessageBox.Show("Please enter a custom maintenance type.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (dpScheduled.SelectedDate == null)
            {
                MessageBox.Show("Please select a scheduled date.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(txtIntervalDays.Text, out int d) || d <= 0)
            {
                MessageBox.Show("Interval days must be a positive number.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(txtIntervalMileage.Text, out int m) || m <= 0)
            {
                MessageBox.Show("Interval mileage must be a positive number.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        // ── NAV ───────────────────────────────────────────────────────────
        private void btnNavVehicles_Click(object sender, RoutedEventArgs e)
        {
            //new AdminVehicleView().Show();
            //this.Close();

            //NavigationService.Navigate(new AdminVehicleView());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VVMainWindow vVMainWindow = new VVMainWindow();
            vVMainWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new AdminVehicleView());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new PublicVehicleView());
        }
    }

    // ── Helper classes ────────────────────────────────────────────────────
    public class VehicleComboItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Plate { get; set; }
        public string Name { get; set; }
    }

    public class MaintenanceRecord
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string VehiclePlate { get; set; }
        public string VehicleName { get; set; }
        public string MaintenanceType { get; set; }
        public string CustomType { get; set; }
        public string Status { get; set; }
        public string ScheduledDate { get; set; }
        public string CompletedDate { get; set; }
        public int CurrentMileage { get; set; }
        public int NextMileage { get; set; }
        public int IntervalDays { get; set; }
        public int IntervalMileage { get; set; }
        public double Cost { get; set; }
        public string Notes { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedBy { get; set; }

        public string DisplayType => CustomType != null ? $"Custom: {CustomType}" : MaintenanceType;
        public string CostDisplay => Cost > 0 ? $"LKR {Cost:N2}" : "—";
        public string NextDueDate => DateTime.TryParse(ScheduledDate, out var sd)
                                        ? sd.AddDays(IntervalDays).ToString("dd MMM yyyy") : "—";

        public string DaysLeft
        {
            get
            {
                if (Status == "Completed" || Status == "Cancelled") return "—";
                if (!DateTime.TryParse(ScheduledDate, out var sd)) return "—";
                int days = (sd.Date - DateTime.Today).Days;
                return days < 0 ? $"⚠ {Math.Abs(days)}d late"
                     : days == 0 ? "Today"
                     : $"{days}d";
            }
        }

        public string StatusColor
        {
            get
            {
                if (Status == "Completed" || Status == "Cancelled") return "None";
                if (!DateTime.TryParse(ScheduledDate, out var sd)) return "None";
                int days = (sd.Date - DateTime.Today).Days;
                if (days < 0) return "Red";
                if (days <= 7) return "Orange";
                return "None";
            }
        }
    }
}


/*

MAINTENANCE SYSTEM SUMMARY
==========================

PURPOSE
-------
The system manages vehicle maintenance schedules using:

1. Date-based intervals (e.g., every 90 days)
2. Mileage-based intervals (e.g., every 5,000 km)

It automatically tracks upcoming, overdue, and completed maintenance records.

------------------------------------------------------------
1. NEXT MAINTENANCE DATE
------------------------------------------------------------

Formula:

Next Due Date = Scheduled Date + Interval Days

Example:

Scheduled Date = 20-Jun-2026
Interval Days  = 90

Result:

Next Due Date = 18-Sep-2026

------------------------------------------------------------
2. NEXT MAINTENANCE MILEAGE
------------------------------------------------------------

Formula:

Next Mileage = Current Mileage + Interval Mileage

Example:

Current Mileage  = 50,000 km
Interval Mileage = 5,000 km

Result:

Next Mileage = 55,000 km

------------------------------------------------------------
3. REMINDER SYSTEM
------------------------------------------------------------

The system saves:

- Last Service Date
- Last Service Mileage
- Next Due Date
- Next Due Mileage

This allows maintenance reminders to be generated automatically.

------------------------------------------------------------
4. MARK AS COMPLETED
------------------------------------------------------------

When the user clicks:

Mark as Completed

The system:

1. Changes status to Completed
2. Records completion date
3. Saves an audit log entry
4. Automatically schedules the next maintenance

------------------------------------------------------------
5. AUTO-SCHEDULE FEATURE
------------------------------------------------------------

After completion, a new maintenance record is created automatically.

Example:

Completed Date = 20-Jun-2026
Interval Days  = 90

New Scheduled Date = 18-Sep-2026

Current Mileage = 55,000 km
Next Mileage    = 60,000 km

This creates a continuous maintenance cycle.

------------------------------------------------------------
6. OVERDUE DETECTION
------------------------------------------------------------

If:

Scheduled Date < Today

The maintenance becomes:

Overdue

Example:

Scheduled = 01-Jun-2026
Today     = 20-Jun-2026

Result:

19 days late

------------------------------------------------------------
7. DUE SOON DETECTION
------------------------------------------------------------

If maintenance is due within the next 7 days:

Today <= Scheduled Date <= Today + 7 Days

The system marks it as:

Due Soon

------------------------------------------------------------
8. AUDIT LOG
------------------------------------------------------------

The system records all important changes:

- Record Created
- Record Updated
- Record Deleted
- Status Changed
- Mileage Changed
- Cost Changed

This provides a full maintenance history.

------------------------------------------------------------
FINAL WORKFLOW
------------------------------------------------------------

Step 1:
User creates maintenance record.

Step 2:
System calculates:
- Next Due Date
- Next Due Mileage

Step 3:
Reminder information is saved.

Step 4:
Maintenance becomes Due Soon or Overdue when applicable.

Step 5:
User marks maintenance as Completed.

Step 6:
System automatically creates the next maintenance record.

Example Cycle:

50,000 km -> 55,000 km -> 60,000 km -> 65,000 km

20-Jun -> 18-Sep -> 17-Dec -> 17-Mar

RESULT
------

The system automatically manages recurring vehicle maintenance,
tracks service history, generates reminders, detects overdue
services, and creates future maintenance records without manual
user intervention.

*/