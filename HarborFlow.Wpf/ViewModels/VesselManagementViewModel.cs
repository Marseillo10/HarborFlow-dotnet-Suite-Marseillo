using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class VesselManagementViewModel : INotifyPropertyChanged
    {
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly IWindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<VesselManagementViewModel> _logger;
        private readonly SessionContext _sessionContext;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ObservableCollection<Vessel> Vessels { get; } = new ObservableCollection<Vessel>();
        public ObservableCollection<Vessel> FilteredVessels { get; } = new ObservableCollection<Vessel>();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterVessels();
            }
        }

        private Vessel? _selectedVessel;
        public Vessel? SelectedVessel
        {
            get => _selectedVessel;
            set
            {
                _selectedVessel = value;
                OnPropertyChanged();
                (DeleteVesselCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (EditVesselCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand RefreshVesselsCommand { get; }
        public ICommand AddVesselCommand { get; }
        public ICommand EditVesselCommand { get; }
        public ICommand DeleteVesselCommand { get; }

        public bool CanAddVessel => _sessionContext.CurrentUser?.Role == UserRole.Administrator;
        public bool CanEditVessel => _sessionContext.CurrentUser?.Role == UserRole.Administrator;
        public bool CanDeleteVessel => _sessionContext.CurrentUser?.Role == UserRole.Administrator;

        public VesselManagementViewModel(IVesselTrackingService vesselTrackingService, IWindowManager windowManager, INotificationService notificationService, ILogger<VesselManagementViewModel> logger, SessionContext sessionContext, MainWindowViewModel mainWindowViewModel)
        {
            _vesselTrackingService = vesselTrackingService;
            _windowManager = windowManager;
            _notificationService = notificationService;
            _logger = logger;
            _sessionContext = sessionContext;
            _mainWindowViewModel = mainWindowViewModel;
            RefreshVesselsCommand = new AsyncRelayCommand(_ => LoadVesselsAsync());
            AddVesselCommand = new AsyncRelayCommand(_ => AddVessel(), _ => CanAddVessel);
            EditVesselCommand = new AsyncRelayCommand(_ => EditVessel(), _ => SelectedVessel != null && CanEditVessel);
            DeleteVesselCommand = new AsyncRelayCommand(_ => DeleteVessel(), _ => SelectedVessel != null && CanDeleteVessel);
        }

        public async Task LoadVesselsAsync()
        {
            _mainWindowViewModel.IsLoading = true;
            try
            {
                Vessels.Clear();
                var vessels = await _vesselTrackingService.GetAllVesselsAsync();
                foreach (var vessel in vessels)
                {
                    Vessels.Add(vessel);
                }
                FilterVessels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load vessels.");
                
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        private void FilterVessels()
        {
            FilteredVessels.Clear();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var vessel in Vessels)
                {
                    FilteredVessels.Add(vessel);
                }
            }
            else
            {
                foreach (var vessel in Vessels.Where(v => v.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                {
                    FilteredVessels.Add(vessel);
                }
            }
        }

        private async Task AddVessel()
        {
            var newVessel = new Vessel();
            var dialogResult = _windowManager.ShowVesselEditorDialog(newVessel);
            if (dialogResult == true)
            {
                _mainWindowViewModel.IsLoading = true;
                try
                {
                    await _vesselTrackingService.AddVesselAsync(newVessel);
                    await LoadVesselsAsync();
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add vessel.");
                    
                }
                finally
                {
                    _mainWindowViewModel.IsLoading = false;
                }
            }
        }

        private async Task EditVessel()
        {
            if (SelectedVessel == null) return;
            
            var vesselCopy = (Vessel)SelectedVessel.Clone(); 
            var dialogResult = _windowManager.ShowVesselEditorDialog(vesselCopy);
            if (dialogResult == true)
            {
                _mainWindowViewModel.IsLoading = true;
                try
                {
                    await _vesselTrackingService.UpdateVesselAsync(vesselCopy);
                    await LoadVesselsAsync();
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update vessel.");
                    
                }
                finally
                {
                    _mainWindowViewModel.IsLoading = false;
                }
            }
        }

        private async Task DeleteVessel()
        {
            if (SelectedVessel != null)
            {
                if (_notificationService.ShowConfirmation("Delete Vessel", $"Are you sure you want to delete {SelectedVessel.Name}?"))
                {
                    _mainWindowViewModel.IsLoading = true;
                    try
                    {
                        await _vesselTrackingService.DeleteVesselAsync(SelectedVessel.IMO);
                        await LoadVesselsAsync();
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete vessel.");
                        
                    }
                    finally
                    {
                        _mainWindowViewModel.IsLoading = false;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}