using PressureMonitor.Helpers;
using PressureMonitor.Models;
using PressureMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace PressureMonitor.ViewModels
{
    public class TrialsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private List<Trial> _allTrials = new();
        private SortDirections _sortDirection;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Trial> Trials { get; } = new();
        public ICommand SortCommand { get; }
        public ICommand DeleteCommand { get; }
        public TrialsViewModel (DatabaseService db)
        {
            _db = db;
            SortCommand = new RelayCommand(Sort);
            DeleteCommand = new RelayCommand<Trial>(DeleteTrial);
            LoadTrials();
        }
        private void LoadTrials()
        {
            _allTrials = _db.GetTrialsSortedByDate().ToList();
            RefreshCollection(_allTrials.OrderByDescending(t => t.Date));

        }
        private void Sort()
        {
            var sorted = SelectedSortDirections == SortDirections.Newest
                ? _allTrials.OrderByDescending(t => t.Date)
                : _allTrials.OrderBy(t => t.Date);
            RefreshCollection(sorted);
        }
        private void RefreshCollection(IEnumerable<Trial> source)
        {
            Trials.Clear();
            foreach (var trial in source)
                Trials.Add(trial);
        }
        public SortDirections SelectedSortDirections
        {
            get => _sortDirection;
            set
            {
                _sortDirection = value;
                OnPropertyChanged();
            }
        }
        private void DeleteTrial(Trial trial)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить \"{trial.Name}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _db.DeleteTrial(trial.Id);
            _allTrials.Remove(trial);
            Trials.Remove(Trials.First(t => t.Id == trial.Id));

            MessageBox.Show(
                $"Испытание \"{trial.Name}\" успешно удалено.",
                "Удалено",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        public IEnumerable<SortDirections> SortDirectionsOptions =>
            Enum.GetValues(typeof(SortDirections)).Cast<SortDirections>();
        public void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
