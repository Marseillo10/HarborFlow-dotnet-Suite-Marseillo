using HarborFlow.Core.Models;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using HarborFlow.Wpf.Commands;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace HarborFlow.Wpf.ViewModels
{
    public class NavigationItem
    {
        public string? DisplayName { get; set; }
        public ICommand? Command { get; set; }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly SessionContext _sessionContext;
        private readonly INotificationService _notificationService;
        private readonly IWindowManager _windowManager;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly MapViewModel _mapViewModel;
        private readonly ServiceRequestViewModel _serviceRequestViewModel;
        private readonly VesselManagementViewModel _vesselManagementViewModel;

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
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

        private string _notificationMessage = string.Empty;
        private bool _isNotificationVisible = false;
        private Brush _notificationBackground = Brushes.Red;
        private DispatcherTimer _notificationTimer;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand LogoutCommand { get; }
        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToMapCommand { get; }
        public ICommand NavigateToServiceRequestCommand { get; }
        public ICommand NavigateToVesselManagementCommand { get; }
        public ObservableCollection<NavigationItem> NavigationItems { get; }

        private NavigationItem _selectedNavigationItem;
        public NavigationItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set
            {
                _selectedNavigationItem = value;
                OnPropertyChanged();
                if (_selectedNavigationItem?.Command != null)
                {
                    _selectedNavigationItem.Command.Execute(null);
                }
            }
        }


        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotificationVisible
        {
            get => _isNotificationVisible;
            set
            {
                _isNotificationVisible = value;
                OnPropertyChanged();
            }
        }

        public Brush NotificationBackground
        {
            get => _notificationBackground;
            set
            {
                _notificationBackground = value;
                OnPropertyChanged();
            }
        }

        public bool IsServiceRequestVisible
        {
            get
            {
                if (_sessionContext.CurrentUser == null) return false;
                return _sessionContext.CurrentUser.Role == UserRole.PortOfficer ||
                       _sessionContext.CurrentUser.Role == UserRole.Administrator;
            }
        }

        public string CurrentUserName
        {
            get
            {
                return _sessionContext.CurrentUser?.FullName ?? "Guest";
            }
        }

        public MainWindowViewModel(SessionContext sessionContext, INotificationService notificationService, IWindowManager windowManager, DashboardViewModel dashboardViewModel, MapViewModel mapViewModel, ServiceRequestViewModel serviceRequestViewModel, VesselManagementViewModel vesselManagementViewModel)
        {
            _sessionContext = sessionContext;
            _notificationService = notificationService;
            _windowManager = windowManager;
            _dashboardViewModel = dashboardViewModel;
            _mapViewModel = mapViewModel;
            _serviceRequestViewModel = serviceRequestViewModel;
            _vesselManagementViewModel = vesselManagementViewModel;
            _currentViewModel = _dashboardViewModel; // Initialize non-nullable field

            _notificationService.NotificationRequested += OnNotificationRequested;

            LogoutCommand = new RelayCommand(_ => Logout());
            NavigateToDashboardCommand = new RelayCommand(_ => CurrentViewModel = _dashboardViewModel);
            NavigateToMapCommand = new RelayCommand(_ => CurrentViewModel = _mapViewModel);
            NavigateToServiceRequestCommand = new RelayCommand(_ => CurrentViewModel = _serviceRequestViewModel);
            NavigateToVesselManagementCommand = new RelayCommand(_ => CurrentViewModel = _vesselManagementViewModel);

            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { DisplayName = "Dashboard", Command = NavigateToDashboardCommand },
                new NavigationItem { DisplayName = "Map View", Command = NavigateToMapCommand },
                new NavigationItem { DisplayName = "Vessel Management", Command = NavigateToVesselManagementCommand },
                new NavigationItem { DisplayName = "Service Request", Command = NavigateToServiceRequestCommand }
            };

            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _notificationTimer.Tick += (s, e) => IsNotificationVisible = false;

            // Set initial view
            CurrentViewModel = _dashboardViewModel;
            _selectedNavigationItem = NavigationItems[0];
        }

        private void Logout()
        {
            _sessionContext.CurrentUser = null;
            _windowManager.ShowLoginWindow();
        }

        private void OnNotificationRequested(string message, NotificationType type)
        {
            NotificationMessage = message;
            NotificationBackground = type switch
            {
                NotificationType.Success => Brushes.Green,
                NotificationType.Warning => Brushes.Orange,
                NotificationType.Error => Brushes.Red,
                _ => Brushes.Gray
            };
            IsNotificationVisible = true;
            _notificationTimer.Stop();
            _notificationTimer.Start();
        }

        public void OnLoginSuccess()
        {
            OnPropertyChanged(nameof(IsServiceRequestVisible));
            OnPropertyChanged(nameof(CurrentUserName));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
