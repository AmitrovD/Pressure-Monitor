using PressureMonitor.ViewModels;
using PressureMonitor.Views;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PressureMonitor
{
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();
            var vm = (MainViewModel)DataContext;
            vm.RequestSettingsButtonPosition = callback =>
            {
                var settingsButtonPosition = settingsButton.PointToScreen(new Point(0, 0));
                var settingsButtonHeight = settingsButton.ActualHeight;

                var windowPosition = this.PointToScreen(new Point(0,0));
                var windowWidth = this.ActualWidth;
                var windowHeight = this.ActualHeight;
                callback(settingsButtonPosition, settingsButtonHeight, windowPosition, windowWidth, windowHeight);
            };
        }
    }
}