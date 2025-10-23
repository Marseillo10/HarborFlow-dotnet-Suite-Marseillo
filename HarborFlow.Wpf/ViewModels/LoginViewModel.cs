using HarborFlow.Application.Interfaces;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly IWindowManager _windowManager;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthService authService, IWindowManager windowManager)
        {
            _authService = authService;
            _windowManager = windowManager;
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync(object parameter)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.LoginAsync(Username, Password);
                if (user != null)
                {
                    _windowManager.ShowMainWindow();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                // In a real app, log this exception
                ErrorMessage = "An error occurred during login.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}