using Drive_Smart_2._0.Views.VehicleView.Database;
using System.Collections.Generic;
using System.Windows;

namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class AuditLogWindow : Window
    {
        public AuditLogWindow()
        {
            InitializeComponent();
            LoadAuditLog();
        }

        private void LoadAuditLog()
        {
            var list = new List<AuditEntry>();
            using var conn = MaintenanceDatabase.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT ChangedAt, VehiclePlate, MaintenanceType,
                       FieldChanged, OldValue, NewValue, ChangedBy
                FROM MaintenanceAuditLog
                ORDER BY ChangedAt DESC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(new AuditEntry
                {
                    ChangedAt = reader.GetString(0),
                    VehiclePlate = reader.GetString(1),
                    MaintenanceType = reader.GetString(2),
                    FieldChanged = reader.GetString(3),
                    OldValue = reader.IsDBNull(4) ? "—" : reader.GetString(4),
                    NewValue = reader.IsDBNull(5) ? "—" : reader.GetString(5),
                    ChangedBy = reader.GetString(6)
                });
            dgAudit.ItemsSource = list;
        }
    }

    public class AuditEntry
    {
        public string ChangedAt { get; set; }
        public string VehiclePlate { get; set; }
        public string MaintenanceType { get; set; }
        public string FieldChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
    }
}