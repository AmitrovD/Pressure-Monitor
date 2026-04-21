using PressureMonitor.Helpers;
using PressureMonitor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PressureMonitor.ViewModels
{
    public class RegisterTableViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ModbusRegister> Registers { get; set; } = new();
        public ICommand AddRegisterCommand { get; }
        public ICommand RemoveRegisterCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private ModbusRegister _selectedRegister;
        public ModbusRegister SelectedRegister
        {
            get => _selectedRegister;
            set
            {
                _selectedRegister = value;
                OnPropertyChanged();
            }
        }
        public RegisterTableViewModel()
        {
            AddRegisterCommand = new RelayCommand(AddRegister);
            RemoveRegisterCommand = new RelayCommand<ModbusRegister>(RemoveRegister);

            Registers.Add(new ModbusRegister
            {
                Name = "Давление",
                Address = 0,
                Type = RegisterType.Float
            });
        }

        private void AddRegister()
        {
            Registers.Add(new ModbusRegister
            {
                Name = "Новый регистр",
                Address = 0,
                Type = RegisterType.Float
            });
        }

        private void RemoveRegister(ModbusRegister register)
        {
            if (register != null)
                Registers.Remove(register);
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    }
}
