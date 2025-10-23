using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
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

        public ObservableCollection<Vessel> Vessels { get; } = new ObservableCollection<Vessel>();

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

        public VesselManagementViewModel(IVesselTrackingService vesselTrackingService, IWindowManager windowManager)
        {
            _vesselTrackingService = vesselTrackingService;
            _windowManager = windowManager;
            RefreshVesselsCommand = new AsyncRelayCommand(_ => LoadVesselsAsync());
            AddVesselCommand = new AsyncRelayCommand(_ => AddVessel());
            EditVesselCommand = new AsyncRelayCommand(_ => EditVessel(), _ => SelectedVessel != null);
            DeleteVesselCommand = new AsyncRelayCommand(_ => DeleteVessel(), _ => SelectedVessel != null);
        }

        public async Task LoadVesselsAsync()
        {
            Vessels.Clear();
            var vessels = await _vesselTrackingService.GetAllVesselsAsync();
            foreach (var vessel in vessels)
            {
                Vessels.Add(vessel);
            }
        }

        private async Task AddVessel()
        {
            var newVessel = new Vessel();
            var dialogResult = _windowManager.ShowVesselEditorDialog(newVessel);
            if (dialogResult == true)
            {
                await _vesselTrackingService.AddVesselAsync(newVessel);
                await LoadVesselsAsync();
            }
        }

        private async Task EditVessel()
        {
            if (SelectedVessel == null) return;
            
            // It's better to work on a copy
            var vesselCopy = (Vessel)SelectedVessel.Clone(); 
            var dialogResult = _windowManager.ShowVesselEditorDialog(vesselCopy);
            if (dialogResult == true)
            {
                await _vesselTrackingService.UpdateVesselAsync(vesselCopy);
                await LoadVesselsAsync();
            }
        }

        private async Task DeleteVessel()
        {
            if (SelectedVessel != null)
            {
                // Optional: Show a confirmation dialog here
                await _vesselTrackingService.DeleteVesselAsync(SelectedVessel.IMO);
                await LoadVesselsAsync();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
