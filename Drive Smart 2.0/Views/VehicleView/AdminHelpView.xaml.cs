using System.Windows;
using System.Windows.Controls;

namespace Drive_Smart_2._0.Views.VehicleView
{
    public partial class AdminHelpView : Window  // ← Window, not Page
    {
        public AdminHelpView()
        {
            InitializeComponent();
            LoadSection("GettingStarted");
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                // Update active style
                foreach (var child in spNav.Children)
                {
                    if (child is Button navBtn)
                        navBtn.Style = (Style)FindResource("NavButtonStyle");
                }
                btn.Style = (Style)FindResource("NavButtonActiveStyle");

                txtPageTitle.Text = btn.Content.ToString().Substring(3).Trim(); // strip emoji
                LoadSection(tag);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadSection(string tag)
        {
            spContent.Children.Clear();

            switch (tag)
            {
                case "GettingStarted":
                    AddHeader("Welcome to Drive Smart Admin Panel");
                    AddText("This help guide will walk you through all the features available to administrators.");
                    AddSubheading("Quick Overview");
                    AddTip("Use the sidebar to navigate between different help topics.");
                    AddText("As an admin, you can manage vehicles, upload images, edit records, and more.");
                    break;

                case "Vehicles":
                    AddHeader("Managing Vehicles");
                    AddSubheading("Adding a Vehicle");
                    AddText("Fill in the Brand, Model, Plate Number, Year, Color, and Daily Rate fields, then click Add.");
                    AddSubheading("Removing a Vehicle");
                    AddText("Select a vehicle from the table and click Remove. You will be asked to confirm.");
                    AddSubheading("Toggling Status");
                    AddText("Select a vehicle and click Toggle Status to switch between Available and Unavailable.");
                    break;

                case "Images":
                    AddHeader("Adding Images");
                    AddText("Click the Pick Image button to browse for a vehicle image.");
                    AddTip("Supported formats: JPG, JPEG, PNG, BMP, GIF.");
                    AddText("The image will be saved automatically when you add or update the vehicle.");
                    break;

                case "Edit":
                    AddHeader("Editing Records");
                    AddText("Click any row in the vehicle table to load its details into the form.");
                    AddText("Make your changes and click Modify to save them.");
                    AddTip("You can also change the vehicle image when editing.");
                    break;

                case "Duplicate":
                    AddHeader("Duplicating Vehicles");
                    AddText("Select a vehicle and click Duplicate to create a copy with '_COPY' appended to the plate number.");
                    AddTip("After duplicating, update the plate number to a valid unique value.");
                    break;

                case "Errors":
                    AddHeader("Common Errors");
                    AddSubheading("Duplicate Plate Number");
                    AddText("Each vehicle must have a unique plate number. If you see a duplicate error, change the plate.");
                    AddSubheading("Invalid Plate Format");
                    AddText("Sri Lanka plates follow formats like AB-1234, CAB-1234, or 61-1234. Check the hint below the plate field.");
                    AddSubheading("Invalid Year");
                    AddText("Year must be between 1940 and next year.");
                    AddSubheading("Invalid Daily Rate");
                    AddText("Daily rate must be a positive number.");
                    break;

                case "FAQ":
                    AddHeader("Frequently Asked Questions");
                    AddSubheading("Can I recover a deleted vehicle?");
                    AddText("No. Deletion is permanent. Always double-check before confirming.");
                    AddSubheading("What plate formats are supported?");
                    AddText("Old format: AB-1234 | New format: CAB-1234 | Numeric: 61-1234, 100-1234, 300-1234");
                    AddSubheading("How do I mark a vehicle as unavailable?");
                    AddText("Select it in the table and click Toggle Status.");
                    break;
            }
        }

        // ── Content builder helpers ───────────────────────────────────────

        private void AddHeader(string text)
        {
            var border = new Border
            {
                Background = System.Windows.Media.Brushes.SteelBlue,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20, 16, 20, 16),
                Margin = new Thickness(0, 0, 0, 16)
            };
            border.Child = new TextBlock
            {
                Text = text,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };
            spContent.Children.Add(border);
        }

        private void AddSubheading(string text)
        {
            spContent.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.SteelBlue,
                Margin = new Thickness(0, 14, 0, 8)
            });
        }

        private void AddText(string text)
        {
            spContent.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 13,
                Foreground = System.Windows.Media.Brushes.DimGray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 8)
            });
        }

        private void AddTip(string text)
        {
            var border = new Border
            {
                Background = System.Windows.Media.Brushes.AliceBlue,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(0, 0, 0, 14)
            };
            border.Child = new TextBlock
            {
                Text = "💡 " + text,
                FontSize = 13,
                Foreground = System.Windows.Media.Brushes.SteelBlue,
                TextWrapping = TextWrapping.Wrap
            };
            spContent.Children.Add(border);
        }
    }
}