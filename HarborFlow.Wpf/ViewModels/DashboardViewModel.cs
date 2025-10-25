using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IPortServiceManager _portServiceManager;
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly SessionContext _sessionContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DashboardViewModel> _logger;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IWindowManager _windowManager;

        private int _vesselCount;
        public int VesselCount
        {
            get => _vesselCount;
            set { _vesselCount = value; OnPropertyChanged(); }
        }

        private int _activeServiceRequestCount;
        public int ActiveServiceRequestCount
        {
            get => _activeServiceRequestCount;
            set { _activeServiceRequestCount = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Vessel> _recentVessels = new();
        public ObservableCollection<Vessel> RecentVessels
        {
            get => _recentVessels;
            set { _recentVessels = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServiceRequest> _recentServiceRequests = new();
        public ObservableCollection<ServiceRequest> RecentServiceRequests
        {
            get => _recentServiceRequests;
            set { _recentServiceRequests = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ShowUserProfileCommand { get; }

        public DashboardViewModel(IPortServiceManager portServiceManager, IVesselTrackingService vesselTrackingService, SessionContext sessionContext, INotificationService notificationService, ILogger<DashboardViewModel> logger, MainWindowViewModel mainWindowViewModel, IWindowManager windowManager)
        {
            _portServiceManager = portServiceManager;
            _vesselTrackingService = vesselTrackingService;
            _sessionContext = sessionContext;
            _notificationService = notificationService;
            _logger = logger;
            _mainWindowViewModel = mainWindowViewModel;
            _windowManager = windowManager;

            RefreshCommand = new AsyncRelayCommand(_ => LoadDataAsync());
            ShowUserProfileCommand = new RelayCommand(_ => ShowUserProfile());
        }

        private void ShowUserProfile()
        {
            _windowManager.ShowUserProfileDialog();
        }

        public async Task LoadDataAsync()
        {
            _mainWindowViewModel.IsLoading = true;
            try
            {
                var vessels = await _vesselTrackingService.GetAllVesselsAsync();
                VesselCount = vessels.Count();
                RecentVessels = new ObservableCollection<Vessel>(vessels.OrderByDescending(v => v.UpdatedAt).Take(5));

                if (_sessionContext.CurrentUser != null)
                {
                    var serviceRequests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
                    ActiveServiceRequestCount = serviceRequests.Count(sr => sr.Status != RequestStatus.Completed);
                    RecentServiceRequests = new ObservableCollection<ServiceRequest>(serviceRequests.OrderByDescending(sr => sr.RequestDate).Take(5));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data.");
                _notificationService.ShowNotification("Failed to load dashboard data. Please check your connection and try again.", NotificationType.Error);
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
