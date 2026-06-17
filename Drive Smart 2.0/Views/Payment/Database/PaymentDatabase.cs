using System;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Drive_Smart_2._0.Views.Payment
{
    /// <summary>
    /// Plain data holder for one payment transaction captured on the
    /// payment_details screen.
    /// </summary>
    public class PaymentRecord
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string TelNumber { get; set; }
        public string Address { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public string RentalDuration { get; set; }
        public decimal TotalDue { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    /// <summary>
    /// Owns all access to the dedicated payments database, kept separate
    /// from the rest of the app's data at:
    ///   Views\Payment\Database\Payment.db
    /// </summary>
    public static class PaymentDatabase
    {
        // Resolves to "<project folder>\Views\Payment\Database\Payment.db" -
        // i.e. the same folder you see in Solution Explorer - rather than the
        // build output folder (bin\Debug\...), which is where
        // AppDomain.CurrentDomain.BaseDirectory points and is NOT where you'd
        // go looking for the file.
        private static readonly string DbFolder = Path.Combine(
            GetProjectRoot(), "Views", "Payment", "Database");

        private static readonly string DbPath = Path.Combine(DbFolder, "Payment.db");

        /// <summary>Full path to Payment.db, in case you want to log it or
        /// open it from elsewhere (e.g. the Reports screen).</summary>
        public static string DatabasePath => DbPath;

        private static string ConnectionString => $"Data Source={DbPath}";

        /// <summary>
        /// Walks up from the running exe's folder (bin\Debug\..., bin\Debug\net6.0-windows\...,
        /// etc.) until it finds the folder containing the .csproj file - that's the actual
        /// project root regardless of target framework or build configuration. Falls back to
        /// the exe folder itself if no .csproj is found (e.g. a published/standalone build).
        /// </summary>
        private static string GetProjectRoot()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !dir.GetFiles("*.csproj").Any())
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Creates the Database folder, the Payment.db file, and the
        /// Payments table if any of them don't already exist yet.
        /// Safe to call every time the window is opened.
        /// </summary>
        public static void Initialize()
        {
            Directory.CreateDirectory(DbFolder);

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Payments (
                    PaymentId       INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerId      TEXT NOT NULL,
                    CustomerName    TEXT,
                    TelNumber       TEXT,
                    Address         TEXT,
                    VehicleNumber   TEXT,
                    VehicleModel    TEXT,
                    RentalDuration  TEXT,
                    TotalDue        REAL,
                    PaymentMethod   TEXT,
                    PaymentDate     TEXT NOT NULL
                );";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts one payment row and returns its new PaymentId
        /// (handy as a receipt number).
        /// </summary>
        public static long InsertPayment(PaymentRecord record)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var insert = connection.CreateCommand();
            insert.CommandText = @"
                INSERT INTO Payments
                    (CustomerId, CustomerName, TelNumber, Address,
                     VehicleNumber, VehicleModel, RentalDuration,
                     TotalDue, PaymentMethod, PaymentDate)
                VALUES
                    ($customerId, $customerName, $telNumber, $address,
                     $vehicleNumber, $vehicleModel, $rentalDuration,
                     $totalDue, $paymentMethod, $paymentDate);";

            insert.Parameters.AddWithValue("$customerId", record.CustomerId ?? "");
            insert.Parameters.AddWithValue("$customerName", record.CustomerName ?? "");
            insert.Parameters.AddWithValue("$telNumber", record.TelNumber ?? "");
            insert.Parameters.AddWithValue("$address", record.Address ?? "");
            insert.Parameters.AddWithValue("$vehicleNumber", record.VehicleNumber ?? "");
            insert.Parameters.AddWithValue("$vehicleModel", record.VehicleModel ?? "");
            insert.Parameters.AddWithValue("$rentalDuration", record.RentalDuration ?? "");
            insert.Parameters.AddWithValue("$totalDue", record.TotalDue);
            insert.Parameters.AddWithValue("$paymentMethod", record.PaymentMethod ?? "");
            insert.Parameters.AddWithValue("$paymentDate", record.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            insert.ExecuteNonQuery();

            var lastId = connection.CreateCommand();
            lastId.CommandText = "SELECT last_insert_rowid();";
            return (long)lastId.ExecuteScalar();
        }

        /// <summary>
        /// Returns every payment, every column - used by the Reports screen.
        /// </summary>
        public static DataTable GetAllPayments()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Payments ORDER BY PaymentDate DESC;";

            using var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
}
