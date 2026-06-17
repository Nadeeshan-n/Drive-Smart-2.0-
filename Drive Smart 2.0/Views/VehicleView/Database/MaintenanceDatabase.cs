using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows;

namespace Drive_Smart_2._0.Views.VehicleView.Database
{

    public class MaintenanceDatabase
    {
        private static readonly string ProjectRoot = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")
        );

        private static readonly string DbFolder = Path.Combine(
            ProjectRoot, "Views", "VehicleView", "Database"
        );

        public static readonly string DbPath = Path.Combine(DbFolder, "maintenance.db");

        public static SqliteConnection GetConnection()
            => new SqliteConnection($"Data Source={DbPath}");

        public static void InitializeDatabase()
        {
            try
            {
                Directory.CreateDirectory(DbFolder);
                using var conn = GetConnection();
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    -- Main maintenance records table
                    CREATE TABLE IF NOT EXISTS MaintenanceRecords (
                        Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                        VehicleId           INTEGER NOT NULL,
                        VehiclePlate        TEXT    NOT NULL,
                        VehicleName         TEXT    NOT NULL,
                        MaintenanceType     TEXT    NOT NULL,
                        CustomType          TEXT,
                        Status              TEXT    NOT NULL DEFAULT 'Scheduled',
                        ScheduledDate       TEXT    NOT NULL,
                        CompletedDate       TEXT,
                        CurrentMileage      INTEGER,
                        NextMileage         INTEGER,
                        IntervalDays        INTEGER NOT NULL DEFAULT 90,
                        IntervalMileage     INTEGER NOT NULL DEFAULT 5000,
                        Cost                REAL,
                        Notes               TEXT,
                        CreatedAt           TEXT    NOT NULL,
                        UpdatedAt           TEXT    NOT NULL,
                        CreatedBy           TEXT    NOT NULL DEFAULT 'Admin'
                    );

                    -- Audit log — every change is recorded here
                    CREATE TABLE IF NOT EXISTS MaintenanceAuditLog (
                        Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                        RecordId        INTEGER NOT NULL,
                        VehiclePlate    TEXT    NOT NULL,
                        MaintenanceType TEXT    NOT NULL,
                        FieldChanged    TEXT    NOT NULL,
                        OldValue        TEXT,
                        NewValue        TEXT,
                        ChangedBy       TEXT    NOT NULL DEFAULT 'Admin',
                        ChangedAt       TEXT    NOT NULL
                    );

                    -- Reminder settings per vehicle per type
                    CREATE TABLE IF NOT EXISTS MaintenanceReminders (
                        Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                        VehicleId           INTEGER NOT NULL,
                        VehiclePlate        TEXT    NOT NULL,
                        MaintenanceType     TEXT    NOT NULL,
                        IntervalDays        INTEGER NOT NULL DEFAULT 90,
                        IntervalMileage     INTEGER NOT NULL DEFAULT 5000,
                        LastServiceDate     TEXT,
                        LastServiceMileage  INTEGER,
                        NextDueDate         TEXT,
                        NextDueMileage      INTEGER,
                        IsActive            INTEGER NOT NULL DEFAULT 1,
                        UNIQUE(VehicleId, MaintenanceType)
                    );
                ";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Maintenance DB error:\n" + ex.Message, "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

        }

        public static int GetUrgentCount()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT COUNT(*) FROM MaintenanceRecords
            WHERE Status != 'Completed'
            AND Status != 'Cancelled'
            AND date(ScheduledDate) <= date('now', '+7 days')";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch { return 0; }
        }

    }
}



/* logic for the auto scheduling the maintanance
------------------------------------------------------------- 




Admin clicks ✅ Mark Done
        │
        ▼
Current record → Status = "Completed"
                 CompletedDate = today
                 UpdatedAt = timestamp
        │
        ▼
AutoScheduleNext() runs
        │
        ├── nextDate    = today + IntervalDays (e.g. +90 days)
        ├── nextMileage = NextMileage + IntervalMileage (e.g. +5000 km)
        │
        ▼
New record inserted → Status = "Scheduled"
                      ScheduledDate = nextDate
                      CurrentMileage = old NextMileage
                      NextMileage = nextMileage
                      Notes = "Auto-scheduled from record #5"
        │
        ▼
Message shown → "Next service auto-scheduled for 05 Sep 2025"

        │
        ▼
DataGrid refreshes → new Scheduled row appears



*/