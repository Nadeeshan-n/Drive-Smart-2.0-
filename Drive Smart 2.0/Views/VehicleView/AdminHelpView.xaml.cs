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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Drive_Smart_2._0.Views.VehicleView
{


    /// <summary>
    /// Interaction logic for AdminHelpView.xaml
    /// </summary>




    public partial class AdminHelpView : Page
    {
        public AdminHelpView()
        {
            InitializeComponent();
            LoadSection("GettingStarted");
        }

        // ── Navigation ───────────────────────────────────────────────────────

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var clicked = (Button)sender;
            string tag = clicked.Tag?.ToString() ?? "GettingStarted";

            foreach (var child in spNav.Children)
                if (child is Button btn)
                    btn.Style = (Style)FindResource("NavButtonStyle");

            clicked.Style = (Style)FindResource("NavButtonActiveStyle");

            txtPageTitle.Text = tag switch
            {
                "GettingStarted" => "Getting Started",
                "Vehicles" => "Managing Vehicles",
                "Images" => "Adding Images",
                "Edit" => "Editing Records",
                "Duplicate" => "Duplicating Vehicles",
                "Errors" => "Common Errors & Fixes",
                "FAQ" => "Frequently Asked Questions",
                _ => ""
            };

            LoadSection(tag);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminVehicleView());

            //NavigationService.Navigate(new SomeOtherPage());
        }

        // ── Section Loader ───────────────────────────────────────────────────

        private void LoadSection(string section)
        {
            spContent.Children.Clear();
            switch (section)
            {
                case "GettingStarted": BuildGettingStarted(); break;
                case "Vehicles": BuildVehicles(); break;
                case "Images": BuildImages(); break;
                case "Edit": BuildEdit(); break;
                case "Duplicate": BuildDuplicate(); break;
                case "Errors": BuildErrors(); break;
                case "FAQ": BuildFAQ(); break;
            }
        }

        // ── UI Builders ──────────────────────────────────────────────────────

        private void AddSectionHeader(string title, string subtitle = "")
        {
            var border = new Border { Style = (Style)FindResource("SectionHeaderStyle") };
            var sp = new StackPanel();
            sp.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            });
            if (!string.IsNullOrEmpty(subtitle))
                sp.Children.Add(new TextBlock
                {
                    Text = subtitle,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Color.FromRgb(144, 202, 249)),
                    Margin = new Thickness(0, 4, 0, 0)
                });
            border.Child = sp;
            spContent.Children.Add(border);
        }

        private void AddSubheading(string text)
        {
            spContent.Children.Add(new TextBlock
            {
                Text = text,
                Style = (Style)FindResource("SubheadingStyle")
            });
        }

        private void AddTip(string text)
        {
            var border = new Border { Style = (Style)FindResource("TipStyle") };
            border.Child = new TextBlock
            {
                Text = "💡  " + text,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(13, 71, 161))
            };
            spContent.Children.Add(border);
        }

        private void AddStep(string number, string title, string description)
        {
            var card = new Border { Style = (Style)FindResource("CardStyle"), Padding = new Thickness(16) };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(44) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var circle = new Border
            {
                Width = 36,
                Height = 36,
                CornerRadius = new CornerRadius(18),
                Background = new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                VerticalAlignment = VerticalAlignment.Top
            };
            circle.Child = new TextBlock
            {
                Text = number,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(circle, 0);

            var textSp = new StackPanel { Margin = new Thickness(12, 0, 0, 0) };
            textSp.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(26, 35, 126))
            });
            textSp.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(80, 80, 80)),
                Margin = new Thickness(0, 4, 0, 0)
            });
            Grid.SetColumn(textSp, 1);

            grid.Children.Add(circle);
            grid.Children.Add(textSp);
            card.Child = grid;
            spContent.Children.Add(card);
        }

        private void AddQA(string question, string answer, bool isWarning = false)
        {
            var card = new Border
            {
                Style = (Style)FindResource("CardStyle"),
                BorderBrush = isWarning
                    ? new SolidColorBrush(Color.FromRgb(255, 152, 0))
                    : new SolidColorBrush(Color.FromRgb(220, 228, 245))
            };

            var outerGrid = new Grid();
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var accent = new Border
            {
                Background = isWarning
                    ? new SolidColorBrush(Color.FromRgb(255, 152, 0))
                    : new SolidColorBrush(Color.FromRgb(21, 101, 192)),
                CornerRadius = new CornerRadius(10, 0, 0, 10)
            };
            Grid.SetColumn(accent, 0);

            var innerSp = new StackPanel { Margin = new Thickness(16, 14, 16, 14) };
            innerSp.Children.Add(new TextBlock
            {
                Text = (isWarning ? "⚠  " : "Q:  ") + question,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Foreground = isWarning
                    ? new SolidColorBrush(Color.FromRgb(230, 81, 0))
                    : new SolidColorBrush(Color.FromRgb(21, 101, 192))
            });
            innerSp.Children.Add(new TextBlock
            {
                Text = answer,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                Margin = new Thickness(0, 6, 0, 0)
            });
            Grid.SetColumn(innerSp, 1);

            outerGrid.Children.Add(accent);
            outerGrid.Children.Add(innerSp);
            card.Child = outerGrid;
            spContent.Children.Add(card);
        }

        // ── Section Content ──────────────────────────────────────────────────

        private void BuildGettingStarted()
        {
            AddSectionHeader("🚀 Getting Started", "Learn the basics of the Admin Panel");
            AddTip("Welcome to the Drive Smart Admin Panel. Use the sidebar to navigate between topics.");
            AddSubheading("Overview of the Admin Panel");
            AddStep("1", "Open the Admin Panel", "Click the 'Admin Page' button from the Vehicle Catalog or log in directly. The admin panel is restricted to authorized users only.");
            AddStep("2", "Understand the Layout", "The admin panel has a sidebar for navigation and a main content area showing records in a data grid.");
            AddStep("3", "Toolbar Buttons", "At the top of the vehicle list you will find action buttons: Add New, Edit, Duplicate, Delete, and Help. Each button operates on the selected row.");
            AddStep("4", "Selecting a Record", "Click any row in the data grid to select it. The selected row will be highlighted. Most action buttons require a row to be selected first.");
            AddStep("5", "Saving Changes", "After adding or editing a vehicle, click the Save button in the form dialog. Changes are written to the database immediately.");
            AddSubheading("Quick Reference");
            AddQA("Who can access the Admin Panel?", "Only users with administrator credentials can open the admin panel. Regular customers using the public catalog cannot access it.");
            AddQA("Is there an undo feature?", "Currently there is no undo. Please double-check all changes before saving, especially deletions which are permanent.");
        }

        private void BuildVehicles()
        {
            AddSectionHeader("🚗 Managing Vehicles", "Add, view, and remove vehicles from the fleet");
            AddTip("Always fill in all required fields (Brand, Model, Plate Number, Daily Rate, Status) before saving a new vehicle.");
            AddSubheading("Adding a New Vehicle");
            AddStep("1", "Click 'Add New'", "Press the Add New button in the toolbar. A form dialog will open with empty fields.");
            AddStep("2", "Fill in Vehicle Details", "Enter the Brand, Model, Plate Number, Year, Color, and Daily Rate. All required fields must be filled.");
            AddStep("3", "Set the Status", "Choose either 'Available' or 'Rented' from the Status dropdown. New vehicles should typically be set to Available.");
            AddStep("4", "Add an Image (Optional)", "Click the image area or Browse button to attach a photo of the vehicle.");
            AddStep("5", "Save the Vehicle", "Click Save. The vehicle will appear in the catalog immediately and be visible to customers.");
            AddSubheading("Deleting a Vehicle");
            AddStep("1", "Select the Vehicle", "Click the vehicle row you wish to delete in the data grid.");
            AddStep("2", "Click Delete", "Press the Delete button. A confirmation dialog will appear.");
            AddStep("3", "Confirm Deletion", "Click Yes to permanently remove the vehicle. This action cannot be undone.");
            AddSubheading("Changing Vehicle Status");
            AddQA("How do I mark a vehicle as Rented?", "Select the vehicle, click Edit, change the Status dropdown to 'Rented', and save. The status badge in the catalog updates automatically.");
            AddQA("What happens if I delete a vehicle with active bookings?", "The system will warn you before deletion. Cancel or complete any active bookings before removing a vehicle.");
        }

        private void BuildImages()
        {
            AddSectionHeader("🖼️ Adding Images", "Attach and manage vehicle photos");
            AddTip("Use clear, well-lit photos with a minimum resolution of 800×600 pixels for the best appearance in the catalog.");
            AddSubheading("How to Add an Image");
            AddStep("1", "Open Add or Edit Form", "Images can be added when creating a new vehicle or editing an existing one.");
            AddStep("2", "Click the Image Area", "Click the image placeholder box or the 'Browse Image' button. A file picker dialog will open.");
            AddStep("3", "Select an Image File", "Navigate to the image file on your computer. Supported formats are JPG, JPEG, and PNG.");
            AddStep("4", "Preview the Image", "The selected image will preview inside the form. Verify it shows the correct vehicle before saving.");
            AddStep("5", "Save the Record", "Click Save. The image will appear on the vehicle card in the catalog.");
            AddSubheading("Updating an Existing Image");
            AddStep("1", "Select the vehicle and click Edit", "Open the edit form for the vehicle whose image you want to change.");
            AddStep("2", "Click the current image or Browse button", "This opens the file picker so you can choose a new image.");
            AddStep("3", "Save", "The old image reference is replaced with the new one.");
            AddSubheading("Image FAQ");
            AddQA("What file formats are supported?", "JPG, JPEG, and PNG formats are supported. Avoid BMP or TIFF as these may not display correctly.");
            AddQA("Why is my image not showing in the catalog?", "The image file must remain at the same path it was selected from. If you move or rename the file after saving the image will break. Keep images in a stable folder such as /Assets/Images/ inside the project.");
            AddQA("Is there a file size limit?", "There is no hard limit, but keep images under 2MB for good performance. Large images slow down the catalog when many vehicles are loaded.");
            AddQA("Can I remove an image without deleting the vehicle?", "Yes. Open the Edit form, clear the image field, then save. The vehicle will show a gray placeholder instead.");
        }

        private void BuildEdit()
        {
            AddSectionHeader("✏️ Editing Records", "Modify vehicle information after it has been saved");
            AddTip("Select a row first before clicking Edit, otherwise the button will have no effect.");
            AddSubheading("How to Edit a Vehicle");
            AddStep("1", "Select the Row", "Click the vehicle you want to edit in the data grid. It will highlight to confirm selection.");
            AddStep("2", "Click the Edit Button", "Press the Edit button in the toolbar. The edit form opens pre-filled with the vehicle's current data.");
            AddStep("3", "Make Your Changes", "Update any fields — brand, model, year, color, daily rate, status, or image.");
            AddStep("4", "Save or Cancel", "Click Save to apply your changes, or Cancel to discard them and close the form.");
            AddSubheading("Field Reference");
            AddQA("Brand & Model", "Enter the manufacturer name (e.g. Toyota) and model name (e.g. Corolla) separately. They display combined in the catalog.");
            AddQA("Plate Number", "Must be unique. If you enter a plate number already used by another vehicle the system will show a duplicate error.");
            AddQA("Daily Rate", "Enter a numeric value only (e.g. 4500.00). Do not include currency symbols — LKR is appended automatically.");
            AddQA("Status", "Set to 'Available' when the vehicle is ready for rental, or 'Rented' when it is currently out with a customer.");
            AddQA("Year", "Enter a four-digit year (e.g. 2021). Leaving this blank will display 0 in the catalog.");
        }

        private void BuildDuplicate()
        {
            AddSectionHeader("📋 Duplicating Vehicles", "Create a copy of an existing vehicle record");
            AddTip("Always update the Plate Number after duplicating — it must be unique for every vehicle in the system.");
            AddSubheading("How to Duplicate a Vehicle");
            AddStep("1", "Select the Vehicle to Copy", "Click the row of the vehicle you want to duplicate in the data grid.");
            AddStep("2", "Click the Duplicate Button", "Press the Duplicate button in the toolbar. A new form opens pre-filled with all the same data.");
            AddStep("3", "Change the Plate Number", "The plate number must be unique. Update it to the new vehicle's actual plate before saving.");
            AddStep("4", "Update Other Different Fields", "Change any fields that differ such as color, year, or image.");
            AddStep("5", "Save the New Vehicle", "Click Save. The duplicate is saved as a brand-new record and the original remains unchanged.");
            AddSubheading("Duplication FAQ");
            AddQA("Does duplicating copy the image too?", "Yes, the image path is copied. If the vehicles share the same photo this is fine. If they differ, browse for a new image after duplicating.");
            AddQA("Can I duplicate a vehicle that is currently Rented?", "Yes, but make sure to set the new vehicle's status to 'Available' before saving.");
            AddQA("What if I forget to change the Plate Number?", "The system will detect the duplicate plate and show an error. You cannot save until the plate number is unique.");
        }

        private void BuildErrors()
        {
            AddSectionHeader("⚠️ Common Errors & Fixes", "Troubleshoot problems in the admin panel");
            AddSubheading("Database & Connection Errors");
            AddQA("Unable to open database connection", "The SQLite database file may be missing or the path has changed. Verify the file exists in the project folder and restart the application.", isWarning: true);
            AddQA("Database is locked", "Another instance of the application may be running and holding the database open. Close all other instances of Drive Smart and try again.", isWarning: true);
            AddSubheading("Validation Errors");
            AddQA("Plate Number already exists", "Each vehicle must have a unique plate number. Check the value you entered and correct it. This also appears after duplicating if you forgot to update the plate.", isWarning: true);
            AddQA("Required fields are missing", "Brand, Model, Plate Number, Daily Rate, and Status are all required. Make sure none of these fields are blank before saving.", isWarning: true);
            AddQA("Invalid Daily Rate", "The Daily Rate field accepts numbers only. Remove any letters, currency symbols, or spaces and enter a plain number such as 3500 or 3500.50.", isWarning: true);
            AddSubheading("Image Errors");
            AddQA("Image shows as gray placeholder", "The image file has been moved, renamed, or deleted. Re-edit the vehicle and re-select the image from its new location, then save.", isWarning: true);
            AddQA("Cannot load image file", "The file may be corrupted or in an unsupported format. Convert the image to JPG or PNG using any image editor and re-add it.", isWarning: true);
            AddSubheading("General Issues");
            AddQA("Clicking Edit or Delete has no effect", "No row is selected. Click a vehicle row first to select it, then click Edit or Delete.", isWarning: true);
            AddQA("Changes are not appearing in the public catalog", "Close and reopen the Vehicle Catalog window. The catalog loads data fresh each time it is opened.", isWarning: true);
        }

        private void BuildFAQ()
        {
            AddSectionHeader("❓ Frequently Asked Questions", "Quick answers to common questions");
            AddQA("How many vehicles can the system store?", "There is no hard limit. Performance may slow beyond several thousand records, but for typical fleet sizes the system runs smoothly.");
            AddQA("Can two admins use the system at the same time?", "It is recommended that only one admin makes changes at a time to avoid database lock conflicts with the local SQLite database.");
            AddQA("How do I back up the vehicle data?", "Locate the SQLite database file in the project folder and copy it to a safe location regularly. Keep a dated backup folder.");
            AddQA("Can I export the vehicle list to Excel or PDF?", "This feature is not currently built in. You can use DB Browser for SQLite to export data from the database directly.");
            AddQA("How do I reset a vehicle's status after a rental ends?", "Select the vehicle, click Edit, change Status from 'Rented' to 'Available', and save. The catalog reflects the change immediately.");
            AddQA("Why does the app open slowly sometimes?", "On first launch the application loads all vehicle images from disk. Compress images to under 500KB for faster load times.");
            AddQA("Is the data stored locally or in the cloud?", "By default the system uses a local SQLite database. A cloud database option is available in the code but requires configuration of the connection string.");
            AddTip("Still stuck? Contact your system administrator or developer for further assistance with Drive Smart v2.0.");
        }
    }
}