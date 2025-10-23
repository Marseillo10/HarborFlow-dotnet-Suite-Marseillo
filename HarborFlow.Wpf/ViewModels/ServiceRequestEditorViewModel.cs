using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class ServiceRequestEditorViewModel : INotifyPropertyChanged
    {
        private ServiceRequest _serviceRequest;
        public ServiceRequest ServiceRequest
        {
            get => _serviceRequest;
            set
            {
                _serviceRequest = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? CloseRequested;

        public ServiceRequestEditorViewModel(ServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest; // Initialize the non-nullable field
            ServiceRequest = serviceRequest;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
