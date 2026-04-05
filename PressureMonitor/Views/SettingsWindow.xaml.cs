using PressureMonitor.ViewModels;
using System.Windows;

namespace PressureMonitor.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
        }
        public double WindowWidth => Width;
        public double WindowHeight => Height;
    }
}
