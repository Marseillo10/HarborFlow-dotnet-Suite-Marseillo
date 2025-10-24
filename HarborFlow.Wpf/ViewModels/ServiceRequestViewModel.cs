using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
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
        private readonly SessionContext _sessionContext;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ObservableCollection<ServiceRequest> ServiceRequests { get; } = new ObservableCollection<ServiceRequest>();

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

        public bool CanUserApproveOrReject => CanApproveOrReject();
        public bool CanAddServiceRequest => _sessionContext.CurrentUser?.Role == UserRole.MaritimeAgent;
        public bool CanEditServiceRequest => _sessionContext.CurrentUser?.Role == UserRole.MaritimeAgent && SelectedServiceRequest?.RequestedBy == _sessionContext.CurrentUser?.UserId;
        public bool CanDeleteServiceRequest => _sessionContext.CurrentUser?.Role == UserRole.Administrator;


        public ServiceRequestViewModel(IPortServiceManager portServiceManager, IWindowManager windowManager, SessionContext sessionContext, MainWindowViewModel mainWindowViewModel)
        {
            _portServiceManager = portServiceManager;
            _windowManager = windowManager;
            _sessionContext = sessionContext;
            _mainWindowViewModel = mainWindowViewModel;
            RefreshServiceRequestsCommand = new AsyncRelayCommand(_ => LoadServiceRequestsAsync());
            AddServiceRequestCommand = new AsyncRelayCommand(_ => AddServiceRequest(), _ => CanAddServiceRequest);
            EditServiceRequestCommand = new AsyncRelayCommand(_ => EditServiceRequest(), _ => SelectedServiceRequest != null && CanEditServiceRequest);
            DeleteServiceRequestCommand = new AsyncRelayCommand(_ => DeleteServiceRequest(), _ => SelectedServiceRequest != null && CanDeleteServiceRequest);
            ApproveServiceRequestCommand = new AsyncRelayCommand(_ => ApproveServiceRequest(), _ => CanApproveOrReject());
            RejectServiceRequestCommand = new AsyncRelayCommand(_ => RejectServiceRequest(), _ => CanApproveOrReject());
        }

        private bool CanApproveOrReject()
        {
            if (SelectedServiceRequest == null || _sessionContext.CurrentUser == null)
                return false;

            return _sessionContext.CurrentUser.Role == UserRole.Administrator || _sessionContext.CurrentUser.Role == UserRole.PortOfficer;
        }

        public async Task LoadServiceRequestsAsync()
        {
            if (_sessionContext.CurrentUser == null) return;

            _mainWindowViewModel.IsLoading = true;
            try
            {
                ServiceRequests.Clear();
                var requests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
                foreach (var request in requests)
                {
                    ServiceRequests.Add(request);
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
                catch (Exception)
                {
                    // In a real app, log this exception
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
                catch (Exception)
                {
                    // In a real app, log this exception
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
                _mainWindowViewModel.IsLoading = true;
                try
                {
                    // Optional: Show a confirmation dialog here
                    await _portServiceManager.DeleteServiceRequestAsync(SelectedServiceRequest.RequestId);
                    await LoadServiceRequestsAsync();
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
            catch (Exception)
            {
                // In a real app, log this exception
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        private async Task RejectServiceRequest()
        {
            if (SelectedServiceRequest == null || _sessionContext.CurrentUser == null) return;
            _mainWindowViewModel.IsLoading = true;
            try
            {
                // In a real app, you would show a dialog to get the rejection reason
                await _portServiceManager.RejectServiceRequestAsync(SelectedServiceRequest.RequestId, _sessionContext.CurrentUser.UserId, "Rejected from WPF App");
                await LoadServiceRequestsAsync();
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