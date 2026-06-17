using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Drive_Smart_2._0.Views.Payment
{
    /// <summary>
    /// Interaction logic for PAYMENT_END___PRINT_BILL.xaml
    /// </summary>
    public partial class PAYMENT_END___PRINT_BILL : Window
    {
        private PaymentRecord _record;
        private long _receiptNumber;

        public PAYMENT_END___PRINT_BILL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens the bill screen pre-filled with the payment that was just saved.
        /// </summary>
        public PAYMENT_END___PRINT_BILL(PaymentRecord record, long receiptNumber) : this()
        {
            _record = record;
            _receiptNumber = receiptNumber;

            ReceiptNumberText.Text = receiptNumber.ToString();
            PaymentDateText.Text = record.PaymentDate.ToString("yyyy-MM-dd HH:mm");
            CustomerIdText.Text = record.CustomerId;
            CustomerNameText.Text = record.CustomerName;
            VehicleText.Text = $"{record.VehicleNumber} ({record.VehicleModel})";
            RentalDurationText.Text = record.RentalDuration;
            PaymentMethodText.Text = record.PaymentMethod;
            TotalDueText.Text = record.TotalDue.ToString("N2");
        }

        private void PrintInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
                return;

            // WPF's print pipeline can render a blank page for any visual that has
            // a live Effect applied (ReceiptCard uses DropShadowEffect via the
            // CardBorder style). Strip the effect off, force a layout pass so the
            // printed snapshot reflects the effect-free state, print, then restore
            // it so the on-screen UI is unaffected.
            var originalEffect = ReceiptCard.Effect;
            ReceiptCard.Effect = null;
            ReceiptCard.UpdateLayout();

            try
            {
                printDialog.PrintVisual(ReceiptCard, $"Receipt {_receiptNumber}");
            }
            finally
            {
                ReceiptCard.Effect = originalEffect;
            }
        }

        private void BackToDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Assumes the app's MainWindow hosts the dashboard and stays open in
            // the background while payment screens are shown on top of it, so
            // "back to dashboard" = close every other window and bring it forward.
            // If Dashboard is actually its own Window class, swap this body for:
            //     new Dashboard().Show();
            //     this.Close();
            foreach (var window in Application.Current.Windows.OfType<Window>().ToList())
            {
                if (window != Application.Current.MainWindow)
                    window.Close();
            }

            Application.Current.MainWindow?.Activate();
        }
    }
}