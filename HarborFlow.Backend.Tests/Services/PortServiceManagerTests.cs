using System;
using System.Linq;
using System.Threading.Tasks;
using HarborFlow.Application.Services;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace HarborFlow.Backend.Tests.Services
{
    public class PortServiceManagerTests
    {
        private HarborFlowDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new HarborFlowDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private ILogger<PortServiceManager> GetLogger()
        {
            return new Mock<ILogger<PortServiceManager>>().Object;
        }

        private INotificationHub GetNotificationHub()
        {
            return new Mock<INotificationHub>().Object;
        }

        private ISynchronizationService GetSyncService()
        {
            return new Mock<ISynchronizationService>().Object;
        }

        [Fact]
        public async Task SubmitServiceRequestAsync_ShouldCreateAndStoreRequest()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(SubmitServiceRequestAsync_ShouldCreateAndStoreRequest));
            var manager = new PortServiceManager(dbContext, GetLogger(), GetSyncService(), GetNotificationHub());
            var newRequest = new ServiceRequest
            {
                VesselImo = "1234567",
                ServiceType = ServiceType.Bunkering,
                RequestDate = DateTime.UtcNow
            };

            // Act
            var result = await manager.SubmitServiceRequestAsync(newRequest);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(RequestStatus.Submitted);
            result.RequestId.Should().NotBeEmpty();

            var requestInDb = await dbContext.ServiceRequests.FindAsync(result.RequestId);
            requestInDb.Should().NotBeNull();
            requestInDb.VesselImo.Should().Be("1234567");
        }

        [Fact]
        public async Task ApproveServiceRequestAsync_ShouldChangeStatusToApproved()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(ApproveServiceRequestAsync_ShouldChangeStatusToApproved));
            var manager = new PortServiceManager(dbContext, GetLogger(), GetSyncService(), GetNotificationHub());
            var approverId = Guid.NewGuid();
            var request = new ServiceRequest { VesselImo = "123", Status = RequestStatus.Submitted };
            dbContext.ServiceRequests.Add(request);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await manager.ApproveServiceRequestAsync(request.RequestId, approverId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(RequestStatus.Approved);

            var historyInDb = await dbContext.ApprovalHistories.FirstOrDefaultAsync(h => h.RequestId == request.RequestId);
            historyInDb.Should().NotBeNull();
            historyInDb.Action.Should().Be(ApprovalAction.Approve);
            historyInDb.ApprovedBy.Should().Be(approverId);
        }

        [Fact]
        public async Task RejectServiceRequestAsync_ShouldChangeStatusToRejected()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(RejectServiceRequestAsync_ShouldChangeStatusToRejected));
            var manager = new PortServiceManager(dbContext, GetLogger(), GetSyncService(), GetNotificationHub());
            var rejectorId = Guid.NewGuid();
            var reason = "Invalid documentation";
            var request = new ServiceRequest { VesselImo = "123", Status = RequestStatus.Submitted };
            dbContext.ServiceRequests.Add(request);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await manager.RejectServiceRequestAsync(request.RequestId, rejectorId, reason);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(RequestStatus.Rejected);
            result.Notes.Should().Be(reason);

            var historyInDb = await dbContext.ApprovalHistories.FirstOrDefaultAsync(h => h.RequestId == request.RequestId);
            historyInDb.Should().NotBeNull();
            historyInDb.Action.Should().Be(ApprovalAction.Reject);
            historyInDb.ApprovedBy.Should().Be(rejectorId);
        }

        [Fact]
        public async Task GetAllServiceRequestsAsync_ForAdmin_ShouldReturnAllRequests()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetAllServiceRequestsAsync_ForAdmin_ShouldReturnAllRequests));
            var manager = new PortServiceManager(dbContext, GetLogger(), GetSyncService(), GetNotificationHub());
            var adminUser = new User { UserId = Guid.NewGuid(), Role = UserRole.Administrator };
            var user1Id = Guid.NewGuid();

            dbContext.ServiceRequests.AddRange(
                new ServiceRequest { VesselImo = "1", RequestedBy = user1Id },
                new ServiceRequest { VesselImo = "2", RequestedBy = Guid.NewGuid() }
            );
            await dbContext.SaveChangesAsync();

            // Act
            var result = await manager.GetAllServiceRequestsAsync(adminUser);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllServiceRequestsAsync_ForAgent_ShouldReturnOnlyOwnedRequests()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetAllServiceRequestsAsync_ForAgent_ShouldReturnOnlyOwnedRequests));
            var manager = new PortServiceManager(dbContext, GetLogger(), GetSyncService(), GetNotificationHub());
            var agentUser = new User { UserId = Guid.NewGuid(), Role = UserRole.MaritimeAgent };

            dbContext.ServiceRequests.AddRange(
                new ServiceRequest { VesselImo = "1", RequestedBy = agentUser.UserId },
                new ServiceRequest { VesselImo = "2", RequestedBy = Guid.NewGuid() },
                new ServiceRequest { VesselImo = "3", RequestedBy = agentUser.UserId }
            );
            await dbContext.SaveChangesAsync();

            // Act
            var result = await manager.GetAllServiceRequestsAsync(agentUser);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.RequestedBy == agentUser.UserId);
        }
    }
}
