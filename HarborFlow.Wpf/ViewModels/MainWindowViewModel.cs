using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.Net.NetworkInformation;

namespace HarborFlow.Wpf.ViewModels
{
    public class NavigationItem
    {
        public string? DisplayName { get; set; }
        public ICommand? Command { get; set; }
        public bool IsVisible { get; set; } = true;
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly SessionContext _sessionContext;
        private readonly IWindowManager _windowManager;
        private readonly ISettingsService _settingsService;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly MapViewModel _mapViewModel;
        private readonly ServiceRequestViewModel _serviceRequestViewModel;
        private readonly VesselManagementViewModel _vesselManagementViewModel;
        private readonly NewsViewModel _newsViewModel;

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

        public bool IsLoggedIn => _sessionContext.CurrentUser != null;
        public bool IsGuest => !IsLoggedIn;

        private string _notificationMessage = string.Empty;
        private bool _isNotificationVisible = false;
        private Brush _notificationBackground = Brushes.Red;
        private DispatcherTimer _notificationTimer;
        private DispatcherTimer _onlineStatusTimer;

        private PathGeometry _themeIcon;
        public PathGeometry ThemeIcon
        {
            get => _themeIcon;
            set
            {
                _themeIcon = value;
                OnPropertyChanged();
            }
        }

        private string _onlineStatus = string.Empty;
        public string OnlineStatus
        {
            get => _onlineStatus;
            set
            {
                _onlineStatus = value;
                OnPropertyChanged();
            }
        }

        private Brush _onlineStatusColor = Brushes.Green;
        public Brush OnlineStatusColor
        {
            get => _onlineStatusColor;
            set
            {
                _onlineStatusColor = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand LogoutCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToMapCommand { get; }
        public ICommand NavigateToServiceRequestCommand { get; }
        public ICommand NavigateToVesselManagementCommand { get; }
        public ICommand NavigateToNewsCommand { get; }
        public ICommand ShowUserProfileCommand { get; }
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

        public string CurrentUserName
        {
            get
            {
                return _sessionContext.CurrentUser?.FullName ?? "Guest";
            }
        }

        public MainWindowViewModel(SessionContext sessionContext, IWindowManager windowManager, ISettingsService settingsService, DashboardViewModel dashboardViewModel, MapViewModel mapViewModel, ServiceRequestViewModel serviceRequestViewModel, VesselManagementViewModel vesselManagementViewModel, NewsViewModel newsViewModel, INotificationHub notificationHub)
        {
            _sessionContext = sessionContext;
            _windowManager = windowManager;
            _settingsService = settingsService;
            _dashboardViewModel = dashboardViewModel;
            _mapViewModel = mapViewModel;
            _serviceRequestViewModel = serviceRequestViewModel;
            _vesselManagementViewModel = vesselManagementViewModel;
            _newsViewModel = newsViewModel;
            _currentViewModel = _dashboardViewModel; // Initialize non-nullable field
            _themeIcon = new PathGeometry();

            notificationHub.NotificationReceived += OnNotificationReceived;
            _sessionContext.UserChanged += SessionContext_UserChanged;

            LogoutCommand = new RelayCommand(_ => _sessionContext.CurrentUser = null); // Simplified logout
            LoginCommand = new RelayCommand(_ => _windowManager.ShowLoginWindow());
            RegisterCommand = new RelayCommand(_ => _windowManager.ShowRegisterWindow());
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
            NavigateToDashboardCommand = new RelayCommand(_ => CurrentViewModel = _dashboardViewModel);
            NavigateToMapCommand = new RelayCommand(_ => CurrentViewModel = _mapViewModel);
            NavigateToServiceRequestCommand = new RelayCommand(_ => CurrentViewModel = _serviceRequestViewModel);
            NavigateToVesselManagementCommand = new RelayCommand(_ => CurrentViewModel = _vesselManagementViewModel);
            NavigateToNewsCommand = new RelayCommand(_ => CurrentViewModel = _newsViewModel);
            ShowUserProfileCommand = new RelayCommand(_ => _windowManager.ShowUserProfileDialog());

            NavigationItems = new ObservableCollection<NavigationItem>();

            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _notificationTimer.Tick += (s, e) => IsNotificationVisible = false;

            _onlineStatusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _onlineStatusTimer.Tick += (s, e) => CheckOnlineStatus();
            _onlineStatusTimer.Start();

            // Set initial view and state
            UpdateNavigation();
            UpdateThemeIcon(App.Theme);
        }

        private void SessionContext_UserChanged()
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsGuest));
            OnPropertyChanged(nameof(CurrentUserName));
            UpdateNavigation();
        }

        private void UpdateNavigation()
        {
            NavigationItems.Clear();

            if (IsGuest)
            {
                NavigationItems.Add(new NavigationItem { DisplayName = "Map View", Command = NavigateToMapCommand });
                NavigationItems.Add(new NavigationItem { DisplayName = "Maritime News", Command = NavigateToNewsCommand });
                NavigationItems.Add(new NavigationItem { DisplayName = "Login", Command = LoginCommand });
                NavigationItems.Add(new NavigationItem { DisplayName = "Register", Command = RegisterCommand });
                CurrentViewModel = _mapViewModel; // Default view for guests
            }
            else
            {
                NavigationItems.Add(new NavigationItem { DisplayName = "Dashboard", Command = NavigateToDashboardCommand });
                NavigationItems.Add(new NavigationItem { DisplayName = "Map View", Command = NavigateToMapCommand });
                NavigationItems.Add(new NavigationItem { DisplayName = "Maritime News", Command = NavigateToNewsCommand });

                if (_sessionContext.CurrentUser?.Role == UserRole.Administrator)
                {
                    NavigationItems.Add(new NavigationItem { DisplayName = "Vessel Management", Command = NavigateToVesselManagementCommand });
                }
                if (_sessionContext.CurrentUser?.Role == UserRole.Administrator || _sessionContext.CurrentUser?.Role == UserRole.PortOfficer)
                {
                    NavigationItems.Add(new NavigationItem { DisplayName = "Service Requests", Command = NavigateToServiceRequestCommand });
                }
                NavigationItems.Add(new NavigationItem { DisplayName = "Logout", Command = LogoutCommand });
                CurrentViewModel = _dashboardViewModel; // Default view for logged in users
            }

            if (NavigationItems.Any())
            {
                SelectedNavigationItem = NavigationItems.First();
            }
        }

        private void CheckOnlineStatus()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                OnlineStatus = "Online";
                OnlineStatusColor = Brushes.Green;
            }
            else
            {
                OnlineStatus = "Offline";
                OnlineStatusColor = Brushes.Red;
            }
        }

        private void ToggleTheme()
        {
            var app = (App)System.Windows.Application.Current;
            var newTheme = App.Theme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
            app.SetTheme(newTheme);
            _settingsService.SetTheme(newTheme);
            UpdateThemeIcon(newTheme);
        }

        private void UpdateThemeIcon(ThemeType theme)
        {
            var iconKey = theme == ThemeType.Light ? "MoonIcon" : "SunIcon";
            ThemeIcon = (PathGeometry)System.Windows.Application.Current.FindResource(iconKey);
        }

        private void OnNotificationReceived(string message, NotificationType type)
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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}