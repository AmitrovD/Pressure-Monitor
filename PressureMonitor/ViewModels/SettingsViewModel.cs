using PressureMonitor.Models;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PressureMonitor.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private GenerationType _selectedType;
        private PressureDirections _direction;
        private string _textBoxValue = string.Empty;
        private string _staticLimit;
        private string _linearStartValue;
        private string _linearStep;
        private string _randomLimit;
        private string _registerAddress = "0";
        private readonly AppSettings _appSettings = new();
        private string _ipAddress = "127.0.0.1";
        private string _port = "502";
        private string _slaveId = "1";

        public event Action<GenerationType>? SelectedTypeChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? SettingsChanged;

        public SettingsViewModel() { }
        public string TextBoxValue
        {
            get => _textBoxValue;
            set
            {
                if (TextBoxValue == null)
                {
                    MessageBox.Show("Поле не было заполенно числом!");
                    return;
                }
                if (!double.TryParse(value, out double result))
                {
                    MessageBox.Show("Введите корректное число!");
                    return;
                }
                if (result < 0)
                {
                    MessageBox.Show("Число не может быть отрицательным!");
                }
                
                _textBoxValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsValueValid));
            }
        }
        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); SettingsChanged?.Invoke(); }
        }

        public string Port
        {
            get => _port;
            set { _port = value; OnPropertyChanged(); SettingsChanged?.Invoke(); }
        }

        public string SlaveId
        {
            get => _slaveId;
            set { _slaveId = value; OnPropertyChanged(); SettingsChanged?.Invoke(); }
        }

        private bool IsValueValid =>
            double.TryParse(TextBoxValue, out double result) && result >= 0;

        public GenerationType SelectedType
        {
            get => _selectedType;
            set 
            { 
                _selectedType = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(StaticVisibility));
                OnPropertyChanged(nameof(ModbusVisibility));
                OnPropertyChanged(nameof(LinearVisibility));
                OnPropertyChanged(nameof(DirectionVisibility));
                SelectedTypeChanged?.Invoke(_selectedType);
            }
        }
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public IEnumerable<PressureDirections> PressureDirections =>
            Enum.GetValues(typeof(PressureDirections)).Cast<PressureDirections>();

        public PressureDirections SelectedDirection
        {
            get => _direction;
            set
            {
                _direction = value;
                OnPropertyChanged();
            }
        }
        

        public Visibility StaticVisibility =>
            SelectedType == GenerationType.Static ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ModbusVisibility =>
            SelectedType == GenerationType.Modbus ? Visibility.Visible : Visibility.Collapsed;

        public Visibility LinearVisibility =>
            SelectedType == GenerationType.Linear ? Visibility.Visible : Visibility.Collapsed;

        public Visibility DirectionVisibility =>
            SelectedType == GenerationType.Linear ? Visibility.Visible : Visibility.Collapsed;

        public string StaticValue
        {
            get => _staticLimit;
            set
            {
                _staticLimit = value;
                OnPropertyChanged();
                SettingsChanged?.Invoke();
            }
        }
        public string RandomValue
        {
            get => _randomLimit;
            set
            {
                _randomLimit = value;
                OnPropertyChanged();
                SettingsChanged?.Invoke();
            }
        }
        public string LinearStartValue
        {
            get => _linearStartValue;
            set
            {
                _linearStartValue= value;
                OnPropertyChanged();
                SettingsChanged?.Invoke();
            }
        }
        public string LinearStep
        {
            get => _linearStep;
            set
            {
                _linearStep= value;
                OnPropertyChanged();
                SettingsChanged?.Invoke();
            }
        }

        public double? GetTextBoxValue(String str)
        {
            if (double.TryParse(str, out double result) && result >= 0)
                return result;
            return null;
        }

        public AppSettings ToAppSettings()
        {
            return new AppSettings
            {
                SelectedType = SelectedType,
                StaticValue = GetTextBoxValue(StaticValue),
                RandomValue = GetTextBoxValue(RandomValue),
                LinearStartValue = GetTextBoxValue(LinearStartValue),
                LinearStep = GetTextBoxValue(LinearStep),
                SelectedDirection = SelectedDirection,

            };
        }
        public void ApplyFrom(AppSettings settings)
        {
            SelectedType = settings.SelectedType;
            StaticValue = (settings.StaticValue ?? 100).ToString();
            RandomValue = (settings.RandomValue ?? 150).ToString();
            LinearStartValue = (settings.LinearStartValue ?? 0).ToString();
            LinearStep = (settings.LinearStep ?? 1).ToString();
            SelectedDirection = settings.SelectedDirection;
        }
        public ModbusSettings GetModbusSettings()
        {
            var settings = new ModbusSettings();
            settings.IpAddress = IpAddress;

            if (int.TryParse(Port, out int port))
                settings.Port = port;

            if (byte.TryParse(SlaveId, out byte slaveId))
                settings.SlaveId = slaveId;

            return settings;
        }
        public string RegisterAddress
        {
            get => _registerAddress;
            set { _registerAddress = value; OnPropertyChanged(); SettingsChanged?.Invoke(); }
        }
        public ushort GetRegisterAddress()
        {
            if (ushort.TryParse(RegisterAddress, out ushort address))
                return address;
            return 0;
        }
    }
}
