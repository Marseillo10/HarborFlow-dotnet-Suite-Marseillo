using FluentValidation.Results;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Validators;
using HarborFlow.Wpf.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class VesselEditorViewModel : ValidatableViewModelBase
    {
        private readonly VesselValidator _validator;
        private Vessel _vessel;
        public Vessel Vessel
        {
            get => _vessel;
            set
            {
                _vessel = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? CloseRequested;

        public VesselEditorViewModel(Vessel vessel, VesselValidator validator)
        {
            _vessel = vessel;
            _validator = validator;
            Vessel = vessel;
            
            SaveCommand = new RelayCommand(_ => Save(), _ => !HasErrors);
            CancelCommand = new RelayCommand(_ => Cancel());

            Validate();
        }

        private void Validate()
        {
            ClearErrors(nameof(Vessel));
            ValidationResult result = _validator.Validate(Vessel);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    AddError(nameof(Vessel) + "." + error.PropertyName, error.ErrorMessage);
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
}
