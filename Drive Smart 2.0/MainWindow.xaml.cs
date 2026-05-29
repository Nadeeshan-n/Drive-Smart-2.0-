using Drive_Smart_2._0.Views.Auth;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Drive_Smart_2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            systemstartButton.Click += systemstartButton_Click;
        }

        //click start button --. open login window 
        private void systemstartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked!");
        }
    }
}