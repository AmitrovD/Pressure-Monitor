using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PressureMonitor.Models;
using PressureMonitor.Services;
using PressureMonitor.Views;
using PressureMonitor.Helpers;
using OxyPlot;
using System.Runtime.CompilerServices;

namespace PressureMonitor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly EmulationService _emulation;
        private readonly SettingsService _settingsService;
        private readonly DatabaseService _databaseService;
        private readonly ModbusService _modbusService = new();
        private int _secondsElapsed = 0;

        private double _maxValue = double.MinValue;
        private double _minValue = double.MaxValue;
        private double _sumValue = 0;
        private int _pointsCount = 0;

        private string _maxDisplay = "—";
        private string _minDisplay = "—";
        private string _avgDisplay = "—";
        private string _connectionStatusText = "Отключено";
        private string _connectionStatusColor = "Red";

        public event PropertyChangedEventHandler? PropertyChanged;
        public SettingsViewModel Settings { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenTrialCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public PressureChartModel Сhart { get; }
        public Action<Action<Point, double, Point, double, double>>? RequestSettingsButtonPosition { get; set; }

        public string MaxDisplay { get => _maxDisplay; set { _maxDisplay = value; OnPropertyChanged(); } }
        public string MinDisplay { get => _minDisplay; set { _minDisplay = value; OnPropertyChanged(); } }
        public string AvgDisplay { get => _avgDisplay; set { _avgDisplay = value; OnPropertyChanged(); } }
        public string ConnectionStatusText { get => _connectionStatusText; set { _connectionStatusText = value; OnPropertyChanged(); } }
        public string ConnectionStatusColor { get => _connectionStatusColor; set { _connectionStatusColor = value; OnPropertyChanged(); } }
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
            _modbusService.OnValueReceived += HandleModbusValue;
            _modbusService.OnStatusChanged += HandleStatusChanged;
        }
        private void HandleModbusValue(double value)
        {
            if (value > _maxValue) _maxValue = value;
            if (value < _minValue) _minValue = value;
            _sumValue += value;
            _pointsCount++;

            App.Current.Dispatcher.Invoke(() =>
            {
                MaxDisplay = $"Макс: {_maxValue:F2}";
                MinDisplay = $"Мин: {_minValue:F2}";
                AvgDisplay = $"Среднее: {(_sumValue / _pointsCount):F2}";

                Сhart.AddPoint(_secondsElapsed, value);
                _secondsElapsed++;
            });
        }

        private void HandleStatusChanged(ConnectionStatus status)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (status == ConnectionStatus.Connected)
                {
                    ConnectionStatusText = "Подключено";
                    ConnectionStatusColor = "Green";
                }
                else if (status == ConnectionStatus.Disconnected)
                {
                    ConnectionStatusText = "Отключено";
                    ConnectionStatusColor = "Red";
                }
                else if (status == ConnectionStatus.Timeout)
                {
                    ConnectionStatusText = "Таймаут";
                    ConnectionStatusColor = "Orange";
                }
                else
                {
                    ConnectionStatusText = "Ошибка";
                    ConnectionStatusColor = "Red";
                }
            });
        }
        private async void Start()
        {
            Сhart.Clear();
            _secondsElapsed = 0;
            _maxValue = double.MinValue;
            _minValue = double.MaxValue;
            _sumValue = 0;
            _pointsCount = 0;
            MaxDisplay = "—";
            MinDisplay = "—";
            AvgDisplay = "—";
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
                    _emulation.StartGeneration(staticLimit.Value);
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
                    _emulation.StartGeneration(0);
                    break;
                case GenerationType.Modbus:
                    var modbusSettings = Settings.GetModbusSettings();
                    _modbusService.SetSettings(modbusSettings);
                    await _modbusService.StartAsync(Settings.GetRegisterAddress());
                    break;
            }
        }
        private void Stop()
        {
            _emulation.StopGeneration();
            _modbusService.Stop();

            var points = Сhart.GetPoints();

            if (points.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения");
                return;
            }

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

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
