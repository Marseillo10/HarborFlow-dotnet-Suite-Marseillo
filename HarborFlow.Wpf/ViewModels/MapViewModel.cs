using System.ComponentModel;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HarborFlow.Wpf.Commands;
using System.Linq;
using System;
using HarborFlow.Wpf.Interfaces;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Wpf.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<MapViewModel> _logger;
        private string _searchTerm;
        private ObservableCollection<Vessel> _searchResults;
        private Vessel? _selectedVessel;
        private VesselType? _selectedVesselTypeFilter;
        private bool _isHistoryVisible;
        private string _selectedMapLayer;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<Vessel>? VesselSelected;
        public event EventHandler<IEnumerable<VesselPosition>>? HistoryTrackRequested;
        public event EventHandler<string>? MapLayerChanged;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                _ = UpdateSuggestionsAsync();
            }
        }

        public ObservableCollection<string> Suggestions { get; private set; }
        public ObservableCollection<VesselType> VesselTypeFilters { get; } = new ObservableCollection<VesselType>(Enum.GetValues(typeof(VesselType)).Cast<VesselType>());
        public ObservableCollection<string> MapLayers { get; } = new ObservableCollection<string> { "Street", "Satellite", "Nautical" };

        public VesselType? SelectedVesselTypeFilter
        {
            get => _selectedVesselTypeFilter;
            set
            {
                _selectedVesselTypeFilter = value;
                OnPropertyChanged(nameof(SelectedVesselTypeFilter));
                UpdateFilteredVessels();
            }
        }

        public string SelectedMapLayer
        {
            get => _selectedMapLayer;
            set
            {
                _selectedMapLayer = value;
                OnPropertyChanged(nameof(SelectedMapLayer));
                MapLayerChanged?.Invoke(this, _selectedMapLayer);
            }
        }

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set
            {
                _isHistoryVisible = value;
                OnPropertyChanged(nameof(IsHistoryVisible));
                if (_isHistoryVisible)
                {
                    ShowHistoryTrack();
                }
                else
                {
                    ClearHistoryTrack();
                }
            }
        }

        public ICommand SelectSuggestionCommand { get; }
        public ICommand ToggleHistoryCommand { get; }

        public ObservableCollection<Vessel> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        }

        public Vessel? SelectedVessel
        {
            get => _selectedVessel;
            set
            {
                _selectedVessel = value;
                OnPropertyChanged(nameof(SelectedVessel));
                if (_selectedVessel != null)
                {
                    VesselSelected?.Invoke(this, _selectedVessel);
                    if (IsHistoryVisible)
                    {
                        ShowHistoryTrack();
                    }
                }
            }
        }

        public ObservableCollection<Vessel> FilteredVesselsOnMap { get; } = new ObservableCollection<Vessel>();

        public ICommand SearchVesselsCommand { get; }

        public MapViewModel(IVesselTrackingService vesselTrackingService, INotificationService notificationService, ILogger<MapViewModel> logger)
        {
            _vesselTrackingService = vesselTrackingService;
            _notificationService = notificationService;
            _logger = logger;
            _searchTerm = string.Empty;
            _searchResults = new ObservableCollection<Vessel>();
            Suggestions = new ObservableCollection<string>();
            _selectedMapLayer = MapLayers.First();
            SearchVesselsCommand = new RelayCommand(async _ => await SearchVesselsAsync());
            SelectSuggestionCommand = new RelayCommand(SelectSuggestion);
            ToggleHistoryCommand = new RelayCommand(_ => IsHistoryVisible = !IsHistoryVisible);

            _vesselTrackingService.TrackedVessels.CollectionChanged += OnTrackedVesselsChanged;
            UpdateFilteredVessels();

            // Define a bounding box for Southeast Asia / Indonesia
            var boundingBoxes = new[]
            {
                new[] { -15.0, 90.0 }, // Min (Lat, Lon)
                new[] { 20.0, 150.0 }  // Max (Lat, Lon)
            };

            _ = _vesselTrackingService.StartTracking(boundingBoxes);
        }

        private void OnTrackedVesselsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFilteredVessels();
        }

        private void UpdateFilteredVessels()
        {
            FilteredVesselsOnMap.Clear();
            var filtered = _vesselTrackingService.TrackedVessels
                .Where(v => SelectedVesselTypeFilter == null || v.VesselType == SelectedVesselTypeFilter);
            
            foreach (var vessel in filtered)
            {
                FilteredVesselsOnMap.Add(vessel);
            }
        }

        private void SelectSuggestion(object? suggestion)
        {
            if (suggestion is string selectedVesselName)
            {
                SearchTerm = selectedVesselName;
                Suggestions.Clear();
                _ = SearchVesselsAsync();
            }
        }

        private async Task UpdateSuggestionsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchTerm))
                {
                    Suggestions.Clear();
                    return;
                }

                var results = await _vesselTrackingService.SearchVesselsAsync(SearchTerm);
                Suggestions.Clear();
                foreach (var vessel in results.Take(10)) // Limit suggestions
                {
                    Suggestions.Add(vessel.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load search suggestions.");
                _notificationService.ShowNotification("Failed to load search suggestions.", NotificationType.Error);
            }
        }

        public async Task SearchVesselsAsync()
        {
            try
            {
                var results = await _vesselTrackingService.SearchVesselsAsync(SearchTerm);
                SearchResults = new ObservableCollection<Vessel>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform vessel search.");
                _notificationService.ShowNotification("Failed to perform vessel search.", NotificationType.Error);
            }
        }

        private void ShowHistoryTrack()
        {
            if (SelectedVessel != null)
            {
                HistoryTrackRequested?.Invoke(this, SelectedVessel.Positions);
            }
        }

        private void ClearHistoryTrack()
        {
            HistoryTrackRequested?.Invoke(this, new List<VesselPosition>());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _vesselTrackingService.TrackedVessels.CollectionChanged -= OnTrackedVesselsChanged;
            _vesselTrackingService.StopTracking().Wait();
        }
    }
}
