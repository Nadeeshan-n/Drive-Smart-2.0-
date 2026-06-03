using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows;

namespace Drive_Smart_2._0.Views.VehicleView.Database
{
    public class VehicleDatabase
    {
        // Goes up from bin/Debug/net8.0-windows/ back to the project root
        private static readonly string ProjectRoot = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")
        );

        private static readonly string DbFolder = Path.Combine(
            ProjectRoot, "Views", "VehicleView", "Database"
        );

        public static readonly string DbPath = Path.Combine(DbFolder, "vehicles.db");

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbPath}");
        }

        public static void InitializeDatabase()
        {
            try
            {
                Directory.CreateDirectory(DbFolder);

                using var conn = GetConnection();
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Vehicles (
                        Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                        Brand       TEXT    NOT NULL,
                        Model       TEXT    NOT NULL,
                        PlateNumber TEXT    NOT NULL UNIQUE,
                        Year        INTEGER,
                        Color       TEXT,
                        DailyRate   REAL    NOT NULL,
                        Status      TEXT    NOT NULL DEFAULT 'Available'
                    );";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}