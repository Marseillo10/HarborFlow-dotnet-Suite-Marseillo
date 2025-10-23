using FluentAssertions;
using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class ServiceRequestViewModelTests
    {
        private readonly Mock<IPortServiceManager> _portServiceManagerMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly SessionContext _sessionContext;
        private readonly ServiceRequestViewModel _viewModel;

        public ServiceRequestViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _windowManagerMock = new Mock<IWindowManager>();
            _sessionContext = new SessionContext { CurrentUser = new User { UserId = Guid.NewGuid() } };
            _viewModel = new ServiceRequestViewModel(_portServiceManagerMock.Object, _windowManagerMock.Object, _sessionContext);
        }

        [Fact]
        public async Task LoadServiceRequestsAsync_ShouldClearAndLoadRequests()
        {
            // Arrange
            _viewModel.ServiceRequests.Add(new ServiceRequest());
            var newRequests = new List<ServiceRequest> { new ServiceRequest { RequestId = Guid.NewGuid() }, new ServiceRequest { RequestId = Guid.NewGuid() } };
            _portServiceManagerMock.Setup(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser)).ReturnsAsync(newRequests);

            // Act
            await _viewModel.LoadServiceRequestsAsync();

            // Assert
            _viewModel.ServiceRequests.Should().HaveCount(2);
        }

        [Fact]
        public void EditAndDeleteCommands_ShouldBeDisabled_WhenNoRequestIsSelected()
        {
            // Arrange
            _viewModel.SelectedServiceRequest = null;

            // Act
            var canEdit = _viewModel.EditServiceRequestCommand.CanExecute(null);
            var canDelete = _viewModel.DeleteServiceRequestCommand.CanExecute(null);

            // Assert
            canEdit.Should().BeFalse();
            canDelete.Should().BeFalse();
        }

        [Fact]
        public void EditAndDeleteCommands_ShouldBeEnabled_WhenRequestIsSelected()
        {
            // Arrange
            _viewModel.SelectedServiceRequest = new ServiceRequest();

            // Act
            var canEdit = _viewModel.EditServiceRequestCommand.CanExecute(null);
            var canDelete = _viewModel.DeleteServiceRequestCommand.CanExecute(null);

            // Assert
            canEdit.Should().BeTrue();
            canDelete.Should().BeTrue();
        }

        [Fact]
        public async Task AddServiceRequestCommand_ShouldAddRequest_WhenDialogIsSaved()
        {
            // Arrange
            _windowManagerMock.Setup(w => w.ShowServiceRequestEditorDialog(It.IsAny<ServiceRequest>())).Returns(true);

            // Act
            await (_viewModel.AddServiceRequestCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _portServiceManagerMock.Verify(s => s.SubmitServiceRequestAsync(It.IsAny<ServiceRequest>()), Times.Once);
            _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
        }

        [Fact]
        public async Task EditServiceRequestCommand_ShouldUpdateRequest_WhenDialogIsSaved()
        {
            // Arrange
            var request = new ServiceRequest { RequestId = Guid.NewGuid() };
            _viewModel.SelectedServiceRequest = request;
            _windowManagerMock.Setup(w => w.ShowServiceRequestEditorDialog(It.IsAny<ServiceRequest>())).Returns(true);

            // Act
            await (_viewModel.EditServiceRequestCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _portServiceManagerMock.Verify(s => s.UpdateServiceRequestAsync(It.IsAny<ServiceRequest>()), Times.Once);
            _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
        }
    }
}
