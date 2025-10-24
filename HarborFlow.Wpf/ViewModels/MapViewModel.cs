using System.ComponentModel;
using System.Threading.Tasks;
using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HarborFlow.Wpf.Commands;
using System.Linq;
using System;
using HarborFlow.Wpf.Interfaces;
using System.Collections.Specialized;

namespace HarborFlow.Wpf.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly INotificationService _notificationService;
        private string _searchTerm;
        private ObservableCollection<Vessel> _searchResults;
        private Vessel? _selectedVessel;
        private VesselType? _selectedVesselTypeFilter;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<Vessel>? VesselSelected;

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

        public ICommand SelectSuggestionCommand { get; }

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
                }
            }
        }

        public ObservableCollection<Vessel> FilteredVesselsOnMap { get; } = new ObservableCollection<Vessel>();

        public ICommand SearchVesselsCommand { get; }

        public MapViewModel(IVesselTrackingService vesselTrackingService, INotificationService notificationService)
        {
            _vesselTrackingService = vesselTrackingService;
            _notificationService = notificationService;
            _searchTerm = string.Empty;
            _searchResults = new ObservableCollection<Vessel>();
            Suggestions = new ObservableCollection<string>();
            SearchVesselsCommand = new RelayCommand(async _ => await SearchVesselsAsync());
            SelectSuggestionCommand = new RelayCommand(SelectSuggestion);

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
                _notificationService.ShowNotification("Failed to load search suggestions.");
            }
        }

        public async Task SearchVesselsAsync()
        {
            try
            {
                var results = await _vesselTrackingService.SearchVesselsAsync(SearchTerm);
                SearchResults = new ObservableCollection<Vessel>(results);
            }
            catch (Exception)
            {
                _notificationService.ShowNotification("Failed to perform vessel search.");
            }
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
