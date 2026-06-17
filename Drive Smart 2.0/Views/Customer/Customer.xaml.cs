using Drive_Smart_2._0.Views.Auth.Helpers;
using Drive_Smart_2._0.Views.Customer.Database;
using System;
using System.Windows;
using System.Windows.Controls;


namespace Drive_Smart_2._0.Views.Customer
{
    public partial class Customer : Window
    {
        private readonly CustomerDatabase _db;

        public Customer()
        {
            InitializeComponent();
            _db = new CustomerDatabase();
            SidebarMenu.SetActivePage(ActivePage.Customers);
            // creates DB + Customers table on first run
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ── 1. Read all field values ─────────────────────────────────────
            string name = txtCustomerName.Text.Trim();
            string phone = phonenumber.Text.Trim();
            string address = txtAddress.Text.Trim();
            string gender = chkMale.IsChecked == true ? "Male"
                           : chkFemale.IsChecked == true ? "Female"
                           : string.Empty;
            string email = txtEmail.Text.Trim();
            string nic = identity.Text.Trim();
            string license = txtLicense.Text.Trim();

            // ── 2. Validate ──────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Customer name is required.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (phone.Length != 10 || !long.TryParse(phone, out _))
            {
                MessageBox.Show("Contact number must be exactly 10 digits.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(nic) || nic.Length != 12)
            {
                MessageBox.Show("NIC number must be exactly 12 characters.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── 3. Build the model ───────────────────────────────────────────
            var customer = new CustomerModel
            {
                CustomerName = name,
                ContactNumber = phone,
                Address = address,
                Gender = gender,
                EmailAddress = email,
                NICNumber = nic,
                DrivingLicense = license
            };

            // ── 4. Save to SQLite database ───────────────────────────────────
            try
            {
                long newId = _db.AddCustomer(customer);

                MessageBox.Show(
                    $"Customer saved successfully!\nCustomer ID: {newId}",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to save customer.\n\nError: {ex.Message}",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ── Resets every field after a successful save ───────────────────────
        private void ClearForm()
        {
            txtCustomerName.Text = string.Empty;
            phonenumber.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtEmail.Text = string.Empty;
            identity.Text = string.Empty;
            txtLicense.Text = string.Empty;
            chkMale.IsChecked = false;
            chkFemale.IsChecked = false;
        }

        // ── Keep Male / Female mutually exclusive ────────────────────────────
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == chkMale && chkFemale.IsChecked == true) chkFemale.IsChecked = false;
            if (sender == chkFemale && chkMale.IsChecked == true) chkMale.IsChecked = false;
        }

        // ── Stub handlers (kept so XAML event bindings don't break) ─────────
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_4(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_5(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_6(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_7(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_8(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_9(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_10(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_11(object sender, TextChangedEventArgs e) { }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void txtCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
        }
    }
}