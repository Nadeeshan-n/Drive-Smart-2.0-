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
        private List<CustomerModel> _allCustomers = new List<CustomerModel>();
        private bool _isPanelOpen = false;

        public Admin()
        {
            InitializeComponent();
            _db = new CustomerDatabase();
        }

        // ── Window loaded ────────────────────────────────────────────────────
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        // ── Load / refresh grid ──────────────────────────────────────────────
        private void LoadCustomers()
        {
            try
            {
                DataTable table = _db.GetAllCustomers();
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

        // ── Search ───────────────────────────────────────────────────────────
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter(txtSearch.Text.Trim());

        private void btnSearch_Click(object sender, RoutedEventArgs e)
            => ApplyFilter(txtSearch.Text.Trim());

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

        // ── Refresh ──────────────────────────────────────────────────────────
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadCustomers();
        }

        // ── Delete ───────────────────────────────────────────────────────────
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerModel selected)
            {
                var confirm = MessageBox.Show(
                    $"Delete \"{selected.CustomerName}\" (ID: {selected.CustomerID})?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        _db.DeleteCustomer(selected.CustomerID);
                        LoadCustomers();
                        MessageBox.Show("Customer deleted successfully.", "Deleted",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete.\n\nError: {ex.Message}",
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

        // ── Selection changed ────────────────────────────────────────────────
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomers.SelectedItem is CustomerModel selected)
                txtStatus.Text = $"Selected: {selected.CustomerName}  |  ID: {selected.CustomerID}";
        }

        // ── Toggle panel ─────────────────────────────────────────────────────
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_isPanelOpen) ClosePanel();
            else OpenPanel();
        }

        private void OpenPanel()
        {
            ClearForm();
            MainGrid.ColumnDefinitions[1].Width = new GridLength(340); // FIXED: index instead of name
            AddPanel.Visibility = Visibility.Visible;
            btnAdd.Content = "✕ Close";
            _isPanelOpen = true;
        }

        private void ClosePanel()
        {
            MainGrid.ColumnDefinitions[1].Width = new GridLength(0);   // FIXED: index instead of name
            AddPanel.Visibility = Visibility.Collapsed;
            btnAdd.Content = "+ Add";
            _isPanelOpen = false;
        }

        // ── Save new customer ────────────────────────────────────────────────
        private void btnSaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            txtPanelError.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(txtNewName.Text))
            { txtPanelError.Text = "Full Name is required."; return; }

            if (string.IsNullOrWhiteSpace(txtNewContact.Text))
            { txtPanelError.Text = "Contact Number is required."; return; }

            if (string.IsNullOrWhiteSpace(txtNewNIC.Text))
            { txtPanelError.Text = "NIC Number is required."; return; }

            try
            {
                var newCustomer = new CustomerModel
                {
                    CustomerName = txtNewName.Text.Trim(),
                    ContactNumber = txtNewContact.Text.Trim(),
                    EmailAddress = txtNewEmail.Text.Trim(),
                    NICNumber = txtNewNIC.Text.Trim(),
                    DrivingLicense = txtNewLicense.Text.Trim(),
                    Gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty,
                    Address = txtNewAddress.Text.Trim()
                };

                long newId = _db.AddCustomer(newCustomer);
                LoadCustomers();
                ClosePanel();

                MessageBox.Show($"Customer added successfully. (ID: {newId})", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                txtPanelError.Text = $"Error: {ex.Message}";
            }
        }

        // ── Cancel ───────────────────────────────────────────────────────────
        private void btnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        // ── Clear form ───────────────────────────────────────────────────────
        private void ClearForm()
        {
            txtNewName.Text = string.Empty;
            txtNewContact.Text = string.Empty;
            txtNewEmail.Text = string.Empty;
            txtNewNIC.Text = string.Empty;
            txtNewLicense.Text = string.Empty;
            txtNewAddress.Text = string.Empty;
            cmbGender.SelectedIndex = -1;
            txtPanelError.Text = string.Empty;
        }
    }
}