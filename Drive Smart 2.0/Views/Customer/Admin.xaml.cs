using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Drive_Smart_2._0.Views.Customer.Database;

namespace Drive_Smart_2._0.Views.Customer
{
    public partial class Admin : Window
    {
        private readonly CustomerDatabase _db;

        // Full list kept in memory for client-side search filtering
        private List<CustomerModel> _allCustomers = new List<CustomerModel>();

        public Admin()
        {
            InitializeComponent();
            _db = new CustomerDatabase();
        }

        // ── Load data as soon as the window is visible ───────────────────────
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        // ── Fetch all rows from DB and bind to DataGrid ──────────────────────
        private void LoadCustomers()
        {
            try
            {
                DataTable table = _db.GetAllCustomers();

                // Convert DataTable rows → list of CustomerModel for easy binding
                _allCustomers.Clear();
                foreach (DataRow row in table.Rows)
                {
                    _allCustomers.Add(new CustomerModel
                    {
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        CustomerName = row["CustomerName"]?.ToString() ?? string.Empty,
                        ContactNumber = row["ContactNumber"]?.ToString() ?? string.Empty,
                        Address = row["Address"]?.ToString() ?? string.Empty,
                        Gender = row["Gender"]?.ToString() ?? string.Empty,
                        EmailAddress = row["EmailAddress"]?.ToString() ?? string.Empty,
                        NICNumber = row["NICNumber"]?.ToString() ?? string.Empty,
                        DrivingLicense = row["DrivingLicense"]?.ToString() ?? string.Empty,
                        CreatedAt = row["CreatedAt"]?.ToString() ?? string.Empty
                    });
                }

                dgCustomers.ItemsSource = null;
                dgCustomers.ItemsSource = _allCustomers;

                txtStatus.Text = $"Total records: {_allCustomers.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load customers.\n\nError: {ex.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Live search: filter as the user types ────────────────────────────
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter(txtSearch.Text.Trim());
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter(txtSearch.Text.Trim());
        }

        private void ApplyFilter(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                dgCustomers.ItemsSource = _allCustomers;
                txtStatus.Text = $"Total records: {_allCustomers.Count}";
                return;
            }

            string lower = keyword.ToLower();

            var filtered = _allCustomers.Where(c =>
                (c.CustomerName?.ToLower().Contains(lower) ?? false) ||
                (c.ContactNumber?.ToLower().Contains(lower) ?? false) ||
                (c.NICNumber?.ToLower().Contains(lower) ?? false) ||
                (c.EmailAddress?.ToLower().Contains(lower) ?? false)
            ).ToList();

            dgCustomers.ItemsSource = filtered;
            txtStatus.Text = $"Showing {filtered.Count} of {_allCustomers.Count} records";
        }

        // ── Refresh: reload from DB ──────────────────────────────────────────
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadCustomers();
        }

        // ── Delete selected customer ─────────────────────────────────────────
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerModel selected)
            {
                var confirm = MessageBox.Show(
                    $"Are you sure you want to delete customer:\n\"{selected.CustomerName}\" (ID: {selected.CustomerID})?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeleteCustomer(selected.CustomerID);
                        LoadCustomers(); // refresh grid
                        MessageBox.Show("Customer deleted successfully.", "Deleted",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete customer.\n\nError: {ex.Message}",
                            "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer row to delete.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Row selection changed ────────────────────────────────────────────
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerModel selected)
                txtStatus.Text = $"Selected: {selected.CustomerName}  |  ID: {selected.CustomerID}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddCustomerWindow();
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    // AddCustomer returns the new Customer ID
                    long newCustomerId = _db.AddCustomer(addWindow.NewCustomer);

                    // Refresh the grid to show the new customer
                    LoadCustomers();

                    MessageBox.Show($"Customer added successfully. (ID: {newCustomerId})", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to add customer.\n\nError: {ex.Message}",
                        "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    }
