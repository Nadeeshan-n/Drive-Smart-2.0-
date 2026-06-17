using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Drive_Smart_2._0.Views.Payment;
using Drive_Smart_2._0.Views.VehicleView.Database;
using Drive_Smart_2._0.Views.Customer.Database;

namespace Drive_Smart_2._0.Views.Reports
{
    /// <summary>
    /// Interaction logic for reports.xaml
    /// </summary>
    public partial class reports : Window
    {
        // Same "walk up to the .csproj" trick used in PaymentDatabase.cs, so reports
        // land in the project folder you see in Solution Explorer, not bin\Debug\...
        private static string GetProjectRoot()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !dir.GetFiles("*.csproj").Any())
                dir = dir.Parent;
            return dir?.FullName ?? AppDomain.CurrentDomain.BaseDirectory;
        }

        private static readonly string OutputFolder =
            Path.Combine(GetProjectRoot(), "Views", "Reports", "Output");

        // Remembers the last PDF generated for each report, so PRINT knows what to open.
        private string _lastVehiclePdf;
        private string _lastMaintenancePdf;
        private string _lastCustomerPdf;
        private string _lastPaymentPdf;

        static reports()
        {
            // QuestPDF requires picking a license at startup. Community is free for
            // individuals, students, and small businesses under $1M annual revenue -
            // see questpdf.com/license if that ever stops applying to this project.
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public reports()
        {
            InitializeComponent();
        }

        // ───────────────────────── data pulls ─────────────────────────

        private DataTable RunQuery(SqliteConnection connection, string sql)
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        private DataTable GetVehiclesData()
        {
            VehicleDatabase.InitializeDatabase();
            using var connection = VehicleDatabase.GetConnection();
            return RunQuery(connection, "SELECT * FROM Vehicles");
        }

        private DataTable GetMaintenanceData()
        {
            MaintenanceDatabase.InitializeDatabase();
            using var connection = MaintenanceDatabase.GetConnection();
            return RunQuery(connection, "SELECT * FROM MaintenanceRecords");
        }

        private DataTable GetCustomersData()
        {
            // CustomerDatabase's constructor already runs InitializeDatabase(),
            // and GetAllCustomers() already hands back a ready DataTable.
            return new CustomerDatabase().GetAllCustomers();
        }

        private DataTable GetPaymentsData()
        {
            PaymentDatabase.Initialize();
            return PaymentDatabase.GetAllPayments();
        }

        // ───────────────────────── PDF building ─────────────────────────

        private string BuildPdf(string title, DataTable data, string filePrefix)
        {
            Directory.CreateDirectory(OutputFolder);
            string fileName = $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(OutputFolder, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(8));

                    page.Header().Column(col =>
                    {
                        col.Item().Text(title).FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < data.Columns.Count; i++)
                                columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            foreach (DataColumn dataColumn in data.Columns)
                            {
                                header.Cell().Background(Colors.Blue.Darken2).Padding(4)
                                    .Text(dataColumn.ColumnName).FontColor(Colors.White).Bold();
                            }
                        });

                        bool shaded = false;
                        foreach (DataRow row in data.Rows)
                        {
                            var background = shaded ? Colors.Grey.Lighten4 : Colors.White;
                            foreach (var value in row.ItemArray)
                            {
                                table.Cell().Background(background).Padding(4)
                                    .Text(value?.ToString() ?? "");
                            }
                            shaded = !shaded;
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf(filePath);

            return filePath;
        }

        private string GenerateReport(string title, DataTable data, string filePrefix)
        {
            try
            {
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show($"{title}: there's no data to report on yet.",
                        "Nothing to generate", MessageBoxButton.OK, MessageBoxImage.Information);
                    return null;
                }

                string path = BuildPdf(title, data, filePrefix);
                StatusText.Text = $"{title} saved to:\n{path}";

                var offerSaveAs = MessageBox.Show(
                    $"{title} generated.\n\n{path}\n\nSave a copy somewhere else too?",
                    "Report generated", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (offerSaveAs == MessageBoxResult.Yes)
                {
                    var dialog = new SaveFileDialog
                    {
                        FileName = Path.GetFileName(path),
                        Filter = "PDF files (*.pdf)|*.pdf",
                        InitialDirectory = OutputFolder
                    };
                    if (dialog.ShowDialog() == true)
                        File.Copy(path, dialog.FileName, overwrite: true);
                }

                return path;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not generate {title}:\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void PrintPdf(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show("Generate this report first.", "Nothing to print",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var info = new ProcessStartInfo
                {
                    FileName = path,
                    Verb = "print",
                    UseShellExecute = true
                };
                Process.Start(info);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not print:\n{ex.Message}\n\nMake sure a PDF viewer (Edge, Adobe Reader, etc.) is set as the default app for PDF files.",
                    "Print failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ───────────────────────── button handlers ─────────────────────────

        private void GenerateVehicleReport_Click(object sender, RoutedEventArgs e)
            => _lastVehiclePdf = GenerateReport("Vehicle Report", GetVehiclesData(), "VehicleReport");

        private void PrintVehicleReport_Click(object sender, RoutedEventArgs e)
            => PrintPdf(_lastVehiclePdf);

        private void GenerateMaintenanceReport_Click(object sender, RoutedEventArgs e)
            => _lastMaintenancePdf = GenerateReport("Maintenance Report", GetMaintenanceData(), "MaintenanceReport");

        private void PrintMaintenanceReport_Click(object sender, RoutedEventArgs e)
            => PrintPdf(_lastMaintenancePdf);

        private void GenerateCustomerReport_Click(object sender, RoutedEventArgs e)
            => _lastCustomerPdf = GenerateReport("Customer Report", GetCustomersData(), "CustomerReport");

        private void PrintCustomerReport_Click(object sender, RoutedEventArgs e)
            => PrintPdf(_lastCustomerPdf);

        private void GeneratePaymentReport_Click(object sender, RoutedEventArgs e)
            => _lastPaymentPdf = GenerateReport("Payment Report", GetPaymentsData(), "PaymentReport");

        private void PrintPaymentReport_Click(object sender, RoutedEventArgs e)
            => PrintPdf(_lastPaymentPdf);
    }
}
