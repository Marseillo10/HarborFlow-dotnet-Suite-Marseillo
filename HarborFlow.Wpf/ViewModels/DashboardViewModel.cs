using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HarborFlow.Wpf.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IPortServiceManager _portServiceManager;
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly SessionContext _sessionContext;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private int _vesselCount;
        public int VesselCount
        {
            get => _vesselCount;
            set
            {
                _vesselCount = value;
                OnPropertyChanged();
            }
        }

        private int _activeServiceRequestCount;
        public int ActiveServiceRequestCount
        {
            get => _activeServiceRequestCount;
            set
            {
                _activeServiceRequestCount = value;
                OnPropertyChanged();
            }
        }

        public DashboardViewModel(IPortServiceManager portServiceManager, IVesselTrackingService vesselTrackingService, SessionContext sessionContext, MainWindowViewModel mainWindowViewModel)
        {
            _portServiceManager = portServiceManager;
            _vesselTrackingService = vesselTrackingService;
            _sessionContext = sessionContext;
            _mainWindowViewModel = mainWindowViewModel;
        }

        public async Task LoadDataAsync()
        {
            _mainWindowViewModel.IsLoading = true;
            try
            {
                var vessels = await _vesselTrackingService.GetAllVesselsAsync();
                VesselCount = vessels.Count();

                if (_sessionContext.CurrentUser != null)
                {
                    var serviceRequests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
                    ActiveServiceRequestCount = serviceRequests.Count(sr => sr.Status != RequestStatus.Completed);
                }
            }
            catch (Exception)
            {
                // In a real app, log this exception
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
