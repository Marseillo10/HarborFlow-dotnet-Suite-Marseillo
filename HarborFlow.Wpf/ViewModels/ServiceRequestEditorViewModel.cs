using FluentValidation;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class ServiceRequestEditorViewModel : ValidatableViewModelBase
    {
        private ServiceRequest _serviceRequest;
        public ServiceRequest ServiceRequest
        {
            get => _serviceRequest;
            set
            {
                _serviceRequest = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public ObservableCollection<ServiceType> ServiceTypes { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? CloseRequested;

        public ServiceRequestEditorViewModel(ServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest; // Initialize the non-nullable field
            ServiceRequest = serviceRequest;
            ServiceTypes = new ObservableCollection<ServiceType>(Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>());
            SaveCommand = new RelayCommand(_ => Save(), _ => !HasErrors);
            CancelCommand = new RelayCommand(_ => Cancel());
            Validate();
        }

        private void Validate()
        {
            ClearErrors(nameof(ServiceRequest));
            var validator = new ServiceRequestValidator();
            var result = validator.Validate(ServiceRequest);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    AddError(nameof(ServiceRequest) + "." + error.PropertyName, error.ErrorMessage);
                }
            }
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void Save()
        {
            Validate();
            if (!HasErrors)
            {
                CloseRequested?.Invoke(true);
            }
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }
    }

    public class ServiceRequestValidator : AbstractValidator<ServiceRequest>
    {
        public ServiceRequestValidator()
        {
            RuleFor(sr => sr.VesselImo).NotEmpty().WithMessage("Vessel IMO is required.");
            RuleFor(sr => sr.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(sr => sr.RequestDate).NotEmpty().WithMessage("Request Date is required.");
        }
    }
}
