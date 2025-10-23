using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
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
            }
        }

        public ICommand RefreshServiceRequestsCommand { get; }
        public ICommand AddServiceRequestCommand { get; }
        public ICommand EditServiceRequestCommand { get; }
        public ICommand DeleteServiceRequestCommand { get; }

        public ServiceRequestViewModel(IPortServiceManager portServiceManager, IWindowManager windowManager, SessionContext sessionContext)
        {
            _portServiceManager = portServiceManager;
            _windowManager = windowManager;
            _sessionContext = sessionContext;
            RefreshServiceRequestsCommand = new AsyncRelayCommand(_ => LoadServiceRequestsAsync());
            AddServiceRequestCommand = new AsyncRelayCommand(_ => AddServiceRequest());
            EditServiceRequestCommand = new AsyncRelayCommand(_ => EditServiceRequest(), _ => SelectedServiceRequest != null);
            DeleteServiceRequestCommand = new AsyncRelayCommand(_ => DeleteServiceRequest(), _ => SelectedServiceRequest != null);
        }

        public async Task LoadServiceRequestsAsync()
        {
            if (_sessionContext.CurrentUser == null) return;

            ServiceRequests.Clear();
            var requests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
            foreach (var request in requests)
            {
                ServiceRequests.Add(request);
            }
        }

        private async Task AddServiceRequest()
        {
            if (_sessionContext.CurrentUser == null) return;
            var newRequest = new ServiceRequest { RequestedBy = _sessionContext.CurrentUser.UserId };
            var dialogResult = _windowManager.ShowServiceRequestEditorDialog(newRequest);
            if (dialogResult == true)
            {
                await _portServiceManager.SubmitServiceRequestAsync(newRequest);
                await LoadServiceRequestsAsync();
            }
        }

        private async Task EditServiceRequest()
        {
            if (SelectedServiceRequest == null) return;

            var requestCopy = (ServiceRequest)SelectedServiceRequest.Clone();
            var dialogResult = _windowManager.ShowServiceRequestEditorDialog(requestCopy);
            if (dialogResult == true)
            {
                await _portServiceManager.UpdateServiceRequestAsync(requestCopy);
                await LoadServiceRequestsAsync();
            }
        }

        private Task DeleteServiceRequest()
        {
            if (SelectedServiceRequest != null)
            {
                // In a real app, you would have a Delete method in the service
                // For now, we can't delete.
            }
            return Task.CompletedTask;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}