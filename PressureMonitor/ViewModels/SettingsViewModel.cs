using PressureMonitor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PressureMonitor.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private GenerationType _selectedType;
        private PressureDirections _direction;
        private string _textBoxValue = string.Empty;
        private string _staticLimit = "100";
        private string _linearStartValue = "0";
        private string _linearStep = "1";
        private string _randomLimit = "150";
        private readonly AppSettings _appSettings = new();

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
                OnPropertyChanged(nameof(RandomVisibility));
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

        public Visibility RandomVisibility =>
            SelectedType == GenerationType.Random ? Visibility.Visible : Visibility.Collapsed;

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
            StaticValue = settings.StaticValue.ToString();
            RandomValue = settings.RandomValue.ToString();
            LinearStartValue = settings.LinearStartValue.ToString();
            LinearStep = settings.LinearStep.ToString();
            SelectedDirection = settings.SelectedDirection;
        }

    }
}
