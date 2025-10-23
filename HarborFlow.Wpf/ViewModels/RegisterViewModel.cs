using HarborFlow.Application.Interfaces;
using HarborFlow.Wpf.Commands;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading.Tasks;
using System;

using HarborFlow.Wpf.Validators;
using FluentValidation;
using System.Linq;

namespace HarborFlow.Wpf.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly RegisterViewModelValidator _validator;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _fullName = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isErrorVisible = false;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? RegistrationSucceeded;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                IsErrorVisible = false;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                IsErrorVisible = false;
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                IsErrorVisible = false;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set
            {
                _isErrorVisible = value;
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(IAuthService authService, RegisterViewModelValidator validator)
        {
            _authService = authService;
            _validator = validator;
            RegisterCommand = new RelayCommand(async (_) => await Register(), _ => CanRegister());
        }

        private bool CanRegister()
        {
            // Basic check, detailed validation is done on execution
            return !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password);
        }

        public async Task Register()
        {
            var validationResult = _validator.Validate(this);
            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
                IsErrorVisible = true;
                return;
            }

            if (await _authService.UserExistsAsync(Username))
            {
                ErrorMessage = "Username already exists.";
                IsErrorVisible = true;
                return;
            }

            try
            {
                await _authService.RegisterAsync(Username, Password, Email, FullName);
                IsErrorVisible = false;
                RegistrationSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception)
            {
                ErrorMessage = "An unexpected error occurred during registration.";
                IsErrorVisible = true;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
