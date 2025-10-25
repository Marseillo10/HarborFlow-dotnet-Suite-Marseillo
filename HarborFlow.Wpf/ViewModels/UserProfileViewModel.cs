
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class UserProfileViewModel : ValidatableViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly SessionContext _sessionContext;
        private readonly ILogger<UserProfileViewModel> _logger;

        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        public UserProfileViewModel(IAuthService authService, SessionContext sessionContext, ILogger<UserProfileViewModel> logger)
        {
            _authService = authService;
            _sessionContext = sessionContext;
            _logger = logger;

            UpdateProfileCommand = new AsyncRelayCommand(UpdateProfile, CanUpdateProfile);
            ChangePasswordCommand = new AsyncRelayCommand(ChangePassword, CanChangePassword);

            LoadUserData();
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set { _currentPassword = value; OnPropertyChanged(); }
        }

        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public ICommand UpdateProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        private void LoadUserData()
        {
            if (_sessionContext.CurrentUser != null)
            {
                Username = _sessionContext.CurrentUser.Username;
                Email = _sessionContext.CurrentUser.Email;
            }
        }

        private bool CanUpdateProfile(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Email);
        }

        private async Task UpdateProfile(object? parameter)
        {
            if (_sessionContext.CurrentUser == null) return;

            try
            {
                var userUpdate = new User { UserId = _sessionContext.CurrentUser.UserId, Username = Username, Email = Email };
                var success = await _authService.UpdateUserAsync(userUpdate);
                if (success)
                {
                    _logger.LogInformation("User profile updated successfully.");
                    // Optionally show a success notification
                }
                else
                {
                    _logger.LogWarning("Failed to update user profile.");
                    // Optionally show an error notification
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user profile.");
                // Optionally show an error notification
            }
        }

                private bool CanChangePassword(object? parameter)
                {
                    return !string.IsNullOrWhiteSpace(CurrentPassword) &&
                           !string.IsNullOrWhiteSpace(NewPassword) &&
                           NewPassword == ConfirmPassword;
                }
        
                        private async Task ChangePassword(object? parameter)
                        {
                            if (_sessionContext.CurrentUser == null) return;
                
                            try
                            {                        var success = await _authService.ChangePasswordAsync(_sessionContext.CurrentUser.UserId, CurrentPassword, NewPassword);
                        if (success)
                        {                    _logger.LogInformation("Password changed successfully.");
                    // Optionally show a success notification and clear password fields
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
                else
                {
                    _logger.LogWarning("Failed to change password.");
                    // Optionally show an error notification
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing password.");
                // Optionally show an error notification
            }
        }
    }
}
