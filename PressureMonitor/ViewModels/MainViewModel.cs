using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PressureMonitor.Models;
using PressureMonitor.Services;
using PressureMonitor.Views;
using PressureMonitor.Helpers;
using OxyPlot;
using System.Windows.Media.Media3D;

namespace PressureMonitor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly EmulationService _emulation;
        private readonly SettingsService _settingsService;
        private readonly DatabaseService _databaseService;
        public event PropertyChangedEventHandler? PropertyChanged;
        public SettingsViewModel Settings { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenTrialCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public PressureChartModel Сhart { get; }
        public Action<Action<Point, double, Point, double, double>>? RequestSettingsButtonPosition { get; set; }

        public MainViewModel()
        {
            _emulation = new EmulationService();
            _settingsService = new SettingsService();
            _databaseService = new DatabaseService();

            Settings = new SettingsViewModel();
            Сhart = new PressureChartModel();

            var saved = _settingsService.Load();
            Settings.ApplyFrom(saved);

            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenTrialCommand = new RelayCommand(OpenTrials);
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);

            _emulation.OnDataGenerated += HandleNewPoint;
            Settings.SettingsChanged += () => _settingsService.Save(Settings.ToAppSettings());
        }
        private void Start()
        {
            Сhart.Clear();
            double limit = 100;
            switch (Settings.SelectedType)
            {

                case GenerationType.Static:
                    var staticLimit = Settings.GetTextBoxValue(Settings.StaticValue);

                    if (!staticLimit.HasValue)
                    {
                        MessageBox.Show("Значения были введены неправильно!");
                        break;
                    }
                    _emulation.SetStrategy(new StaticPressureStrategy());
                    limit = staticLimit.Value;
                    break;
                case GenerationType.Linear:
                    var startValue = Settings.GetTextBoxValue(Settings.LinearStartValue);
                    var step = Settings.GetTextBoxValue(Settings.LinearStep);

                    if (!startValue.HasValue || !step.HasValue)
                    {
                        MessageBox.Show("Значения были введены неправильно!");
                        break;
                    }
                    _emulation.SetStrategy(new LinearPressureStrategy((double)startValue, (double)step, Settings.SelectedDirection));
                    break;
                case GenerationType.Random:
                    var randomLimit = Settings.GetTextBoxValue(Settings.StaticValue);

                    if (!randomLimit.HasValue)
                    {
                        MessageBox.Show("Значения были введены неправильно!");
                    }
                    _emulation.SetStrategy(new RandomPressureStrategy());
                    limit = randomLimit.Value;
                    break;
            }
            _emulation.StartGeneration(limit);
        }
        private void Stop()
        {
            _emulation.StopGeneration();

            var points = Сhart.GetPoints();
            MessageBox.Show($"Точек для сохранения: {points.Count}");

            var trial = new Trial
            {
                Name = $"Испытание {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                Date = DateTime.Now,
                Points = points
            };

            _databaseService.SaveTrial(trial);
        }
        public void HandleNewPoint(DataPoint point)
        {
            Сhart.AddPoint((int)point.X, point.Y);
        }
        private void OpenSettings()
        {
            var (buttonPos, buttonHeight, windowPos, windowWidth, windowHeight) = GetButtonPosition();
            var settingsWindow = new SettingsWindow(Settings);

            var position = CalculateSettingsPosition(
                buttonPos, buttonHeight,
                windowPos, windowWidth, windowHeight,
                settingsWindow.WindowWidth, settingsWindow.WindowHeight);

            ShowSettingsWindow(settingsWindow, position);
        }
        private (Point, double, Point, double, double) GetButtonPosition()
        {
            Point buttonPos = new Point(0,0);
            Point windowPos = new Point(0,0);
            double buttonHeight = 0, windowWidth = 0, windowHeight = 0;

            RequestSettingsButtonPosition?.Invoke((bp, bh, wp, ww, wh) =>
            {
                buttonPos = bp; buttonHeight = bh;
                windowPos = wp; windowWidth = ww; windowHeight = wh;
            });

            return (buttonPos, buttonHeight, windowPos, windowWidth, windowHeight);
        }
        private Point CalculateSettingsPosition(
            Point buttonPos, double buttonHeight,
            Point windowPos, double windowWidth, double windowHeight,
            double settingsWidth, double settingsHeight
            )
        {
            double left = buttonPos.X;
            double top = buttonPos.Y;

            if(left + settingsWidth > windowPos.X + windowHeight)
                left = windowPos.X + windowWidth - settingsWidth;

            if(top + settingsHeight > windowPos.Y + windowHeight)
                top = windowPos.Y + - settingsHeight;

            if(left < windowPos.X)
                left = windowPos.X;

            return new Point(left, top);
        }
        private void ShowSettingsWindow(SettingsWindow window, Point position) 
        { 
            window.Left = position.X;
            window.Top = position.Y;
            window.ShowDialog();
        }
        private void OpenTrials()
        {
            var vm = new TrialsViewModel(_databaseService);
            var window = new TrialWindow(vm);
            window.ShowDialog();
        }

    }
}
