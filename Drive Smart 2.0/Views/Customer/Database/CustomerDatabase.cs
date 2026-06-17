using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace Drive_Smart_2._0.Views.Customer.Database
{
    public class CustomerDatabase
    {
        // ──────────────────────────────────────────────────────────────────────
        // Resolves the DB folder to:
        //   <solution_root>\Drive Smart 2.0\Views\Customer\Database\
        //
        // Assembly is at:
        //   <solution_root>\Drive Smart 2.0\bin\Debug\Drive Smart 2.0.exe
        // So we go  ..\..\..\  from bin\Debug  →  Drive Smart 2.0 project root
        // then append  Views\Customer\Database
        // ──────────────────────────────────────────────────────────────────────
        private static readonly string DbFolder = Path.GetFullPath(
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), // bin\Debug
                @"..\..\..",          // up to Drive Smart 2.0 project root
                @"Views\Customer\Database"
            ));

        private static readonly string DbPath = Path.Combine(DbFolder, "DriveSmart.db");

        private string ConnectionString => $"Data Source={DbPath};Version=3;";

        public CustomerDatabase()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // Create the folder if it doesn't exist yet
            if (!Directory.Exists(DbFolder))
                Directory.CreateDirectory(DbFolder);

            // Create the .db file if it doesn't exist yet
            if (!File.Exists(DbPath))
                SQLiteConnection.CreateFile(DbPath);

            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                string createTable = @"
                    CREATE TABLE IF NOT EXISTS Customers (
                        CustomerID      INTEGER PRIMARY KEY AUTOINCREMENT,
                        CustomerName    TEXT    NOT NULL,
                        ContactNumber   TEXT    NOT NULL,
                        Address         TEXT,
                        Gender          TEXT,
                        EmailAddress    TEXT,
                        NICNumber       TEXT    NOT NULL,
                        DrivingLicense  TEXT,
                        CreatedAt       TEXT    DEFAULT (datetime('now','localtime'))
                    );";

                using (var cmd = new SQLiteCommand(createTable, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        public long AddCustomer(CustomerModel customer)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                string insert = @"
                    INSERT INTO Customers
                        (CustomerName, ContactNumber, Address, Gender,
                         EmailAddress, NICNumber, DrivingLicense)
                    VALUES
                        (@Name, @Phone, @Address, @Gender,
                         @Email, @NIC, @License);
                    SELECT last_insert_rowid();";

                using (var cmd = new SQLiteCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@Phone", customer.ContactNumber);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@Gender", customer.Gender);
                    cmd.Parameters.AddWithValue("@Email", customer.EmailAddress);
                    cmd.Parameters.AddWithValue("@NIC", customer.NICNumber);
                    cmd.Parameters.AddWithValue("@License", customer.DrivingLicense);

                    return (long)cmd.ExecuteScalar();
                }
            }
        }

        public DataTable GetAllCustomers()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Customers ORDER BY CreatedAt DESC;";
                using (var adapter = new SQLiteDataAdapter(query, conn))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        public CustomerModel GetCustomerById(int customerId)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Customers WHERE CustomerID = @ID LIMIT 1;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", customerId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapReaderToModel(reader);
                    }
                }
            }
            return null;
        }

        public int UpdateCustomer(CustomerModel customer)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string update = @"
                    UPDATE Customers SET
                        CustomerName   = @Name,
                        ContactNumber  = @Phone,
                        Address        = @Address,
                        Gender         = @Gender,
                        EmailAddress   = @Email,
                        NICNumber      = @NIC,
                        DrivingLicense = @License
                    WHERE CustomerID = @ID;";

                using (var cmd = new SQLiteCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@Phone", customer.ContactNumber);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@Gender", customer.Gender);
                    cmd.Parameters.AddWithValue("@Email", customer.EmailAddress);
                    cmd.Parameters.AddWithValue("@NIC", customer.NICNumber);
                    cmd.Parameters.AddWithValue("@License", customer.DrivingLicense);
                    cmd.Parameters.AddWithValue("@ID", customer.CustomerID);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int DeleteCustomer(int customerId)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string delete = "DELETE FROM Customers WHERE CustomerID = @ID;";
                using (var cmd = new SQLiteCommand(delete, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", customerId);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private CustomerModel MapReaderToModel(SQLiteDataReader reader)
        {
            return new CustomerModel
            {
                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                CustomerName = reader["CustomerName"].ToString(),
                ContactNumber = reader["ContactNumber"].ToString(),
                Address = reader["Address"].ToString(),
                Gender = reader["Gender"].ToString(),
                EmailAddress = reader["EmailAddress"].ToString(),
                NICNumber = reader["NICNumber"].ToString(),
                DrivingLicense = reader["DrivingLicense"].ToString(),
                CreatedAt = reader["CreatedAt"].ToString()
            };
        }

    }
}