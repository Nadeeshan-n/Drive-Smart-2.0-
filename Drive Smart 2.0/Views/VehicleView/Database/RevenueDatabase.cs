using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Drive_Smart_2._0.Views.VehicleView.Database
{
    /// <summary>
    /// One row per completed/active rental charge. This is the single
    /// source of truth for revenue reporting — the dashboard's revenue
    /// chart reads from here instead of guessing from vehicle status.
    /// </summary>
    public class RentalTransaction
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string VehiclePlate { get; set; } = "";
        public string Category { get; set; } = "Car";
        public double Amount { get; set; }
        public string RentalDate { get; set; } = "";   // ISO "yyyy-MM-dd"
    }

    /// <summary>Aggregated point for the monthly trend chart.</summary>
    public class MonthlyRevenuePoint
    {
        public string MonthLabel { get; set; } = "";   // e.g. "Jan 2026"
        public double Total { get; set; }
    }

    /// <summary>Aggregated point for the category breakdown chart.</summary>
    public class CategoryRevenuePoint
    {
        public string Category { get; set; } = "";
        public double Total { get; set; }
    }

    public class RevenueDatabase
    {
        private static readonly string ProjectRoot = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")
        );

        private static readonly string DbFolder = Path.Combine(
            ProjectRoot, "Views", "VehicleView", "Database"
        );

        // Same physical file as VehicleDatabase — keeps everything in one
        // SQLite DB so revenue rows can later be joined against Vehicles.
        public static readonly string DbPath = Path.Combine(DbFolder, "vehicles.db");

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
                    CREATE TABLE IF NOT EXISTS RentalTransactions (
                        Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                        VehicleId    INTEGER NOT NULL,
                        VehiclePlate TEXT    NOT NULL,
                        Category     TEXT    NOT NULL DEFAULT 'Car',
                        Amount       REAL    NOT NULL,
                        RentalDate   TEXT    NOT NULL
                    );";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Revenue DB error:\n" + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Call this whenever a rental is booked/closed out to log real revenue.</summary>
        public static void AddTransaction(int vehicleId, string plate, string category,
            double amount, DateTime? rentalDate = null)
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO RentalTransactions (VehicleId, VehiclePlate, Category, Amount, RentalDate)
                VALUES ($vid, $plate, $cat, $amt, $date)";
            cmd.Parameters.AddWithValue("$vid", vehicleId);
            cmd.Parameters.AddWithValue("$plate", plate);
            cmd.Parameters.AddWithValue("$cat", category);
            cmd.Parameters.AddWithValue("$amt", amount);
            cmd.Parameters.AddWithValue("$date", (rentalDate ?? DateTime.Now).ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();
        }

        /// <summary>Revenue for the last N months (oldest first), zero-filled for empty months.</summary>
        public static List<MonthlyRevenuePoint> GetMonthlyRevenue(int monthsBack = 6)
        {
            var result = new List<MonthlyRevenuePoint>();

            // Build the ordered list of target months first so months with
            // zero transactions still render as a 0 bar/point instead of disappearing.
            var months = new List<DateTime>();
            var anchor = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            for (int i = monthsBack - 1; i >= 0; i--)
                months.Add(anchor.AddMonths(-i));

            var totals = new Dictionary<string, double>();

            try
            {
                using var conn = GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT strftime('%Y-%m', RentalDate) AS ym, SUM(Amount)
                    FROM RentalTransactions
                    WHERE date(RentalDate) >= date($cutoff)
                    GROUP BY ym";
                cmd.Parameters.AddWithValue("$cutoff", months[0].ToString("yyyy-MM-dd"));

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    totals[reader.GetString(0)] = reader.GetDouble(1);
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show("Could not load monthly revenue:\n" + ex.Message);
            }

            foreach (var m in months)
            {
                string key = m.ToString("yyyy-MM");
                totals.TryGetValue(key, out double total);
                result.Add(new MonthlyRevenuePoint
                {
                    MonthLabel = m.ToString("MMM yyyy"),
                    Total = total
                });
            }

            return result;
        }

        /// <summary>Revenue grouped by vehicle category within the last N months.</summary>
        public static List<CategoryRevenuePoint> GetRevenueByCategory(int monthsBack = 6)
        {
            var result = new List<CategoryRevenuePoint>();
            var cutoff = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-(monthsBack - 1));

            try
            {
                using var conn = GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Category, SUM(Amount) AS total
                    FROM RentalTransactions
                    WHERE date(RentalDate) >= date($cutoff)
                    GROUP BY Category
                    ORDER BY total DESC";
                cmd.Parameters.AddWithValue("$cutoff", cutoff.ToString("yyyy-MM-dd"));

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new CategoryRevenuePoint
                    {
                        Category = reader.GetString(0),
                        Total = reader.GetDouble(1)
                    });
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show("Could not load category revenue:\n" + ex.Message);
            }

            return result;
        }

        /// <summary>Sum of revenue within the current calendar month — used for the stat card.</summary>
        public static double GetCurrentMonthRevenue()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT SUM(Amount) FROM RentalTransactions
                    WHERE strftime('%Y-%m', RentalDate) = strftime('%Y-%m', 'now')";
                var val = cmd.ExecuteScalar();
                return (val == DBNull.Value || val == null) ? 0 : Convert.ToDouble(val);
            }
            catch (SqliteException ex)
            {
                MessageBox.Show("Could not load current month revenue:\n" + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Seeds a few months of demo transactions so the chart isn't empty
        /// on a fresh install. Safe to call every startup — it's a no-op if
        /// transactions already exist.
        /// </summary>
        public static void SeedSampleDataIfEmpty()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                var check = conn.CreateCommand();
                check.CommandText = "SELECT COUNT(*) FROM RentalTransactions";
                int count = Convert.ToInt32(check.ExecuteScalar());
                if (count > 0) return;

                var categories = new[] { "Car", "Van", "SUV", "Luxury" };
                var rnd = new Random(42);
                var anchor = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                using var transaction = conn.BeginTransaction();
                for (int m = 5; m >= 0; m--)
                {
                    var monthStart = anchor.AddMonths(-m);
                    int entries = rnd.Next(8, 15);
                    for (int i = 0; i < entries; i++)
                    {
                        var cat = categories[rnd.Next(categories.Length)];
                        double baseRate = cat switch
                        {
                            "Luxury" => 25000,
                            "SUV" => 14000,
                            "Van" => 11000,
                            _ => 8000
                        };
                        double amount = baseRate * rnd.Next(1, 5);
                        var day = rnd.Next(1, DateTime.DaysInMonth(monthStart.Year, monthStart.Month) + 1);
                        var date = new DateTime(monthStart.Year, monthStart.Month, day);

                        var cmd = conn.CreateCommand();
                        cmd.CommandText = @"
                            INSERT INTO RentalTransactions (VehicleId, VehiclePlate, Category, Amount, RentalDate)
                            VALUES ($vid, $plate, $cat, $amt, $date)";
                        cmd.Parameters.AddWithValue("$vid", i + 1);
                        cmd.Parameters.AddWithValue("$plate", $"DEMO-{m}{i}");
                        cmd.Parameters.AddWithValue("$cat", cat);
                        cmd.Parameters.AddWithValue("$amt", amount);
                        cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
            }
            catch (SqliteException ex)
            {
                MessageBox.Show("Could not seed sample revenue data:\n" + ex.Message);
            }
        }
    }
}
