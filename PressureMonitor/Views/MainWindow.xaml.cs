using PressureMonitor.ViewModels;
using System.Windows;

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