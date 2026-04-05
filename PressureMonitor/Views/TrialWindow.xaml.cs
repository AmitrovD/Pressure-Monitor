using PressureMonitor.ViewModels;
using System.Windows;

namespace PressureMonitor.Views
{
    public partial class TrialWindow : Window
    {
        public TrialWindow(TrialsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
