using Drive_Smart_2._0.Views.Auth.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Drive_Smart_2._0.Views.Payment
{
    /// <summary>
    /// Interaction logic for payment_details.xaml
    /// </summary>
    public partial class payment_details : Window
    {
        public payment_details()
        {
            InitializeComponent();


            // Make sure Payment.db (and its Payments table) exist before
            // the user tries to save anything.
            PaymentDatabase.Initialize();
            SidebarMenu.SetActivePage(ActivePage.Payments);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LOOK_UP1_Copy_Click(object sender, RoutedEventArgs e)
        {
            // ---- Validate required fields ----
            if (string.IsNullOrWhiteSpace(CustomerIdTextBox.Text))
            {
                MessageBox.Show("Please enter a Customer ID before proceeding to payment.",
                    "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TotalDueTextBox.Text, out decimal totalDue))
            {
                MessageBox.Show("Total due (LKR) must be a valid number.",
                    "Invalid amount", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (string.IsNullOrWhiteSpace(selectedMethod))
            {
                MessageBox.Show("Please select a payment method.",
                    "Missing information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ---- Build the record from the form ----
            var record = new PaymentRecord
            {
                CustomerId = CustomerIdTextBox.Text.Trim(),
                CustomerName = CustomerNameTextBox.Text.Trim(),
                TelNumber = TelNumberTextBox.Text.Trim(),
                Address = AddressTextBox.Text.Trim(),
                VehicleNumber = VehicleNumberTextBox.Text.Trim(),
                VehicleModel = VehicleModelTextBox.Text.Trim(),
                RentalDuration = RentalDurationTextBox.Text.Trim(),
                TotalDue = totalDue,
                PaymentMethod = selectedMethod,
                PaymentDate = DateTime.Now
            };

            // ---- Save it to Payment.db, then show the success / print-bill screen ----
            try
            {
                long newId = PaymentDatabase.InsertPayment(record);

                var billWindow = new PAYMENT_END___PRINT_BILL(record, newId);
                billWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save the payment:\n{ex.Message}",
                    "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}