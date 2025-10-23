using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class VesselEditorViewModel : INotifyPropertyChanged
    {
        private Vessel _vessel;
        public Vessel Vessel
        {
            get => _vessel;
            set
            {
                _vessel = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool> CloseRequested;

        public VesselEditorViewModel(Vessel vessel)
        {
            Vessel = vessel;
            // In a real app, you might want to work on a copy of the vessel
            // and only apply changes on save.

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        private void Save()
        {
            CloseRequested?.Invoke(true);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
