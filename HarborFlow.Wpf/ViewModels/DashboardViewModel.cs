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
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;

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
        private readonly DispatcherTimer _onlineStatusTimer;

        private readonly NewsViewModel _newsViewModel;

        private int _vesselCount;
        public int VesselCount
        {
            get => _vesselCount;
            set { _vesselCount = value; OnPropertyChanged(); }
        }

        public ObservableCollection<NewsArticle> LatestNews { get; } = new();

        private int _activeServiceRequestCount;
        public int ActiveServiceRequestCount
        {
            get => _activeServiceRequestCount;
            set { _activeServiceRequestCount = value; OnPropertyChanged(); }
        }

        private string _onlineStatus = string.Empty;
        public string OnlineStatus
        {
            get => _onlineStatus;
            set { _onlineStatus = value; OnPropertyChanged(); }
        }

        private Brush _onlineStatusColor = Brushes.Green;
        public Brush OnlineStatusColor
        {
            get => _onlineStatusColor;
            set { _onlineStatusColor = value; OnPropertyChanged(); }
        }

        public SeriesCollection ServiceRequestStatusSeries { get; private set; }
        public SeriesCollection VesselTypeSeries { get; private set; }
        public string[] VesselTypeLabels { get; private set; }
        public SeriesCollection VesselCountrySeries { get; private set; }
        public string[] VesselCountryLabels { get; private set; }
        public SeriesCollection ServiceRequestVesselSeries { get; private set; }
        public string[] ServiceRequestVesselLabels { get; private set; }
        public SeriesCollection ServiceRequestMonthSeries { get; private set; }
        public string[] ServiceRequestMonthLabels { get; private set; }

        public ICommand RefreshCommand { get; }
        public ICommand ShowUserProfileCommand { get; }

        public DashboardViewModel(IPortServiceManager portServiceManager, IVesselTrackingService vesselTrackingService, SessionContext sessionContext, INotificationService notificationService, ILogger<DashboardViewModel> logger, MainWindowViewModel mainWindowViewModel, IWindowManager windowManager, NewsViewModel newsViewModel)
        {
            _portServiceManager = portServiceManager;
            _vesselTrackingService = vesselTrackingService;
            _sessionContext = sessionContext;
            _notificationService = notificationService;
            _logger = logger;
            _mainWindowViewModel = mainWindowViewModel;
            _windowManager = windowManager;
            _newsViewModel = newsViewModel;

            ServiceRequestStatusSeries = new SeriesCollection();
            VesselTypeSeries = new SeriesCollection();
            VesselTypeLabels = Array.Empty<string>();
            VesselCountrySeries = new SeriesCollection();
            VesselCountryLabels = Array.Empty<string>();
            ServiceRequestVesselSeries = new SeriesCollection();
            ServiceRequestVesselLabels = Array.Empty<string>();
            ServiceRequestMonthSeries = new SeriesCollection();
            ServiceRequestMonthLabels = Array.Empty<string>();

            RefreshCommand = new AsyncRelayCommand(_ => LoadDataAsync());
            ShowUserProfileCommand = new RelayCommand(_ => ShowUserProfile());

            _onlineStatusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _onlineStatusTimer.Tick += (s, e) => CheckOnlineStatus();
            _onlineStatusTimer.Start();
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
                UpdateVesselTypeChart(vessels);
                UpdateVesselCountryChart(vessels);

                if (_sessionContext.CurrentUser != null)
                {
                    var serviceRequests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
                    ActiveServiceRequestCount = serviceRequests.Count(sr => sr.Status != RequestStatus.Completed && sr.Status != RequestStatus.Rejected && sr.Status != RequestStatus.Cancelled);
                    UpdateServiceRequestChart(serviceRequests);
                    UpdateServiceRequestVesselChart(serviceRequests, vessels);
                    UpdateServiceRequestMonthChart(serviceRequests);
                }

                await _newsViewModel.LoadNewsCommand.ExecuteAsync(null);
                LatestNews.Clear();
                foreach (var article in _newsViewModel.Articles.Take(5))
                {
                    LatestNews.Add(article);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data.");
                
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        private void UpdateServiceRequestChart(IEnumerable<ServiceRequest> serviceRequests)
        {
            var statusGroups = serviceRequests
                .GroupBy(sr => sr.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToList();

            ServiceRequestStatusSeries.Clear();
            foreach (var group in statusGroups)
            {
                ServiceRequestStatusSeries.Add(new PieSeries
                {
                    Title = group.Status,
                    Values = new ChartValues<int> { group.Count },
                    DataLabels = true
                });
            }
            OnPropertyChanged(nameof(ServiceRequestStatusSeries));
        }

        private void UpdateVesselTypeChart(IEnumerable<Vessel> vessels)
        {
            var typeGroups = vessels
                .GroupBy(v => v.VesselType)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Type)
                .ToList();

            VesselTypeLabels = typeGroups.Select(g => g.Type).ToArray();
            VesselTypeSeries.Clear();
            VesselTypeSeries.Add(new ColumnSeries
            {
                Title = "Vessels",
                Values = new ChartValues<int>(typeGroups.Select(g => g.Count))
            });

            OnPropertyChanged(nameof(VesselTypeLabels));
            OnPropertyChanged(nameof(VesselTypeSeries));
        }

        private void UpdateVesselCountryChart(IEnumerable<Vessel> vessels)
        {
            var countryGroups = vessels
                .GroupBy(v => v.FlagState)
                .Select(g => new { Country = g.Key, Count = g.Count() })
                .OrderBy(g => g.Country)
                .ToList();

            VesselCountryLabels = countryGroups.Select(g => g.Country).ToArray();
            VesselCountrySeries.Clear();
            VesselCountrySeries.Add(new ColumnSeries
            {
                Title = "Vessels",
                Values = new ChartValues<int>(countryGroups.Select(g => g.Count))
            });

            OnPropertyChanged(nameof(VesselCountryLabels));
            OnPropertyChanged(nameof(VesselCountrySeries));
        }

        private void UpdateServiceRequestVesselChart(IEnumerable<ServiceRequest> serviceRequests, IEnumerable<Vessel> vessels)
        {
            var vesselGroups = serviceRequests
                .GroupBy(sr => sr.VesselImo)
                .Select(g => new
                {
                    VesselName = vessels.FirstOrDefault(v => v.IMO == g.Key)?.Name ?? g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderBy(g => g.VesselName)
                .ToList();

            ServiceRequestVesselLabels = vesselGroups.Select(g => g.VesselName).ToArray();
            ServiceRequestVesselSeries.Clear();
            ServiceRequestVesselSeries.Add(new ColumnSeries
            {
                Title = "Service Requests",
                Values = new ChartValues<int>(vesselGroups.Select(g => g.Count))
            });

            OnPropertyChanged(nameof(ServiceRequestVesselLabels));
            OnPropertyChanged(nameof(ServiceRequestVesselSeries));
        }

        private void UpdateServiceRequestMonthChart(IEnumerable<ServiceRequest> serviceRequests)
        {
            var monthGroups = serviceRequests
                .GroupBy(sr => new { sr.RequestDate.Year, sr.RequestDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            ServiceRequestMonthLabels = monthGroups.Select(g => new DateTime(g.Year, g.Month, 1).ToString("MMM yyyy")).ToArray();
            ServiceRequestMonthSeries.Clear();
            ServiceRequestMonthSeries.Add(new ColumnSeries
            {
                Title = "Service Requests",
                Values = new ChartValues<int>(monthGroups.Select(g => g.Count))
            });

            OnPropertyChanged(nameof(ServiceRequestMonthLabels));
            OnPropertyChanged(nameof(ServiceRequestMonthSeries));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
