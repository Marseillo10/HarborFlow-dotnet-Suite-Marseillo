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
    public class ServiceRequestViewModel : INotifyPropertyChanged
    {
        private readonly IPortServiceManager _portServiceManager;
        private readonly IWindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ServiceRequestViewModel> _logger;
        private readonly SessionContext _sessionContext;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ObservableCollection<ServiceRequest> ServiceRequests { get; } = new ObservableCollection<ServiceRequest>();
        public ObservableCollection<ServiceRequest> FilteredServiceRequests { get; } = new ObservableCollection<ServiceRequest>();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterServiceRequests();
            }
        }

        private ServiceRequest? _selectedServiceRequest;
        public ServiceRequest? SelectedServiceRequest
        {
            get => _selectedServiceRequest;
            set
            {
                _selectedServiceRequest = value;
                OnPropertyChanged();
                (EditServiceRequestCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (DeleteServiceRequestCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (ApproveServiceRequestCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (RejectServiceRequestCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(CanUserApproveOrReject));
            }
        }

        public ICommand RefreshServiceRequestsCommand { get; }
        public ICommand AddServiceRequestCommand { get; }
        public ICommand EditServiceRequestCommand { get; }
        public ICommand DeleteServiceRequestCommand { get; }
        public ICommand ApproveServiceRequestCommand { get; }
        public ICommand RejectServiceRequestCommand { get; }

        public bool CanUserApproveOrReject => CanApproveOrReject(null);
        public bool CanAddServiceRequest => _sessionContext.CurrentUser?.Role == UserRole.MaritimeAgent;
        public bool CanEditServiceRequest => _sessionContext.CurrentUser?.Role == UserRole.MaritimeAgent && SelectedServiceRequest?.RequestedBy == _sessionContext.CurrentUser?.UserId;
        public bool CanDeleteServiceRequest => _sessionContext.CurrentUser?.Role == UserRole.Administrator;


        public ServiceRequestViewModel(IPortServiceManager portServiceManager, IWindowManager windowManager, INotificationService notificationService, ILogger<ServiceRequestViewModel> logger, SessionContext sessionContext, MainWindowViewModel mainWindowViewModel)
        {
            _portServiceManager = portServiceManager;
            _windowManager = windowManager;
            _notificationService = notificationService;
            _logger = logger;
            _sessionContext = sessionContext;
            _mainWindowViewModel = mainWindowViewModel;
            RefreshServiceRequestsCommand = new AsyncRelayCommand(_ => LoadServiceRequestsAsync());
            AddServiceRequestCommand = new AsyncRelayCommand(_ => AddServiceRequest(), _ => CanAddServiceRequest);
            EditServiceRequestCommand = new AsyncRelayCommand(_ => EditServiceRequest(), _ => SelectedServiceRequest != null && CanEditServiceRequest);
            DeleteServiceRequestCommand = new AsyncRelayCommand(_ => DeleteServiceRequest(), _ => SelectedServiceRequest != null && CanDeleteServiceRequest);
            ApproveServiceRequestCommand = new AsyncRelayCommand(_ => ApproveServiceRequest(), CanApproveOrReject);
            RejectServiceRequestCommand = new AsyncRelayCommand(_ => RejectServiceRequest(), CanApproveOrReject);
        }

        private bool CanApproveOrReject(object? parameter)
        {
            if (SelectedServiceRequest == null || _sessionContext.CurrentUser == null)
                return false;

            return _sessionContext.CurrentUser.Role == UserRole.Administrator || _sessionContext.CurrentUser.Role == UserRole.PortOfficer;
        }

        public async Task LoadServiceRequestsAsync()
        {
            _mainWindowViewModel.IsLoading = true;
            try
            {
                ServiceRequests.Clear();
                var requests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
                foreach (var request in requests)
                {
                    ServiceRequests.Add(request);
                }
                FilterServiceRequests();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load service requests.");
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        private void FilterServiceRequests()
        {
            FilteredServiceRequests.Clear();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var request in ServiceRequests)
                {
                    FilteredServiceRequests.Add(request);
                }
            }
            else
            {
                foreach (var request in ServiceRequests.Where(r => r.ServiceType.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                {
                    FilteredServiceRequests.Add(request);
                }
            }
        }

        private async Task AddServiceRequest()
        {
            if (_sessionContext.CurrentUser == null) return;
            var newRequest = new ServiceRequest { RequestedBy = _sessionContext.CurrentUser.UserId };
            var dialogResult = _windowManager.ShowServiceRequestEditorDialog(newRequest);
            if (dialogResult == true)
            {
                _mainWindowViewModel.IsLoading = true;
                try
                {
                    await _portServiceManager.SubmitServiceRequestAsync(newRequest);
                    await LoadServiceRequestsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add service request.");
                }
                finally
                {
                    _mainWindowViewModel.IsLoading = false;
                }
            }
        }

        private async Task EditServiceRequest()
        {
            if (SelectedServiceRequest == null) return;

            var requestCopy = (ServiceRequest)SelectedServiceRequest.Clone();
            var dialogResult = _windowManager.ShowServiceRequestEditorDialog(requestCopy);
            if (dialogResult == true)
            {
                _mainWindowViewModel.IsLoading = true;
                try
                {
                    await _portServiceManager.UpdateServiceRequestAsync(requestCopy);
                    await LoadServiceRequestsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update service request.");
                }
                finally
                {
                    _mainWindowViewModel.IsLoading = false;
                }
            }
        }

        private async Task DeleteServiceRequest()
        {
            if (SelectedServiceRequest != null)
            {
                if (_notificationService.ShowConfirmation("Delete Service Request", $"Are you sure you want to delete this service request?"))
                {
                    _mainWindowViewModel.IsLoading = true;
                    try
                    {
                        await _portServiceManager.DeleteServiceRequestAsync(SelectedServiceRequest.RequestId);
                        await LoadServiceRequestsAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete service request.");
                    }
                    finally
                    {
                        _mainWindowViewModel.IsLoading = false;
                    }
                }
            }
        }

        private async Task ApproveServiceRequest()
        {
            if (SelectedServiceRequest == null || _sessionContext.CurrentUser == null) return;
            _mainWindowViewModel.IsLoading = true;
            try
            {
                await _portServiceManager.ApproveServiceRequestAsync(SelectedServiceRequest.RequestId, _sessionContext.CurrentUser.UserId);
                await LoadServiceRequestsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve service request.");
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        private async Task RejectServiceRequest()
        {
            if (SelectedServiceRequest == null || _sessionContext.CurrentUser == null) return;

            var reason = _windowManager.ShowInputDialog("Reject Service Request", "Please provide a reason for rejection:");
            if (reason != null)
            {
                _mainWindowViewModel.IsLoading = true;
                try
                {
                    await _portServiceManager.RejectServiceRequestAsync(SelectedServiceRequest.RequestId, _sessionContext.CurrentUser.UserId, reason);
                    await LoadServiceRequestsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reject service request.");
                }
                finally
                {
                    _mainWindowViewModel.IsLoading = false;
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