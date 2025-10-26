using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using HarborFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HarborFlow.Backend.Tests.Services
{
    public class BookmarkServiceTests
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

        private ILogger<BookmarkService> GetLogger()
        {
            return new Mock<ILogger<BookmarkService>>().Object;
        }

        [Fact]
        public async Task AddBookmarkAsync_ShouldCreateAndStoreBookmark()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(AddBookmarkAsync_ShouldCreateAndStoreBookmark));
            var service = new BookmarkService(dbContext, GetLogger());
            var userId = Guid.NewGuid();
            var newBookmark = new MapBookmark { Name = "Test Location", UserId = userId, North = 10, South = 5, East = 20, West = 15 };

            // Act
            var result = await service.AddBookmarkAsync(newBookmark);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            var bookmarkInDb = await dbContext.MapBookmarks.FindAsync(result.Id);
            bookmarkInDb.Should().NotBeNull();
            bookmarkInDb!.Name.Should().Be("Test Location");
        }

        [Fact]
        public async Task GetBookmarksForUserAsync_ShouldReturnOnlyUserOwnedBookmarks()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetBookmarksForUserAsync_ShouldReturnOnlyUserOwnedBookmarks));
            var service = new BookmarkService(dbContext, GetLogger());
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            await service.AddBookmarkAsync(new MapBookmark { Name = "User1 Bookmark1", UserId = user1Id, North = 1, South = 0, East = 1, West = 0 });
            await service.AddBookmarkAsync(new MapBookmark { Name = "User2 Bookmark", UserId = user2Id, North = 2, South = 1, East = 2, West = 1 });
            await service.AddBookmarkAsync(new MapBookmark { Name = "User1 Bookmark2", UserId = user1Id, North = 3, South = 2, East = 3, West = 2 });

            // Act
            var result = await service.GetBookmarksForUserAsync(user1Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(b => b.UserId == user1Id);
        }

        [Fact]
        public async Task DeleteBookmarkAsync_WithValidOwner_ShouldRemoveBookmark()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(DeleteBookmarkAsync_WithValidOwner_ShouldRemoveBookmark));
            var service = new BookmarkService(dbContext, GetLogger());
            var userId = Guid.NewGuid();
            var bookmark = await service.AddBookmarkAsync(new MapBookmark { Name = "To Delete", UserId = userId, North = 1, South = 0, East = 1, West = 0 });

            // Act
            var result = await service.DeleteBookmarkAsync(bookmark.Id, userId);

            // Assert
            result.Should().BeTrue();
            var bookmarkInDb = await dbContext.MapBookmarks.FindAsync(bookmark.Id);
            bookmarkInDb.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBookmarkAsync_WithInvalidOwner_ShouldNotRemoveBookmark()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(DeleteBookmarkAsync_WithInvalidOwner_ShouldNotRemoveBookmark));
            var service = new BookmarkService(dbContext, GetLogger());
            var ownerId = Guid.NewGuid();
            var attackerId = Guid.NewGuid();
            var bookmark = await service.AddBookmarkAsync(new MapBookmark { Name = "Protected", UserId = ownerId, North = 1, South = 0, East = 1, West = 0 });

            // Act
            var result = await service.DeleteBookmarkAsync(bookmark.Id, attackerId);

            // Assert
            result.Should().BeFalse();
            var bookmarkInDb = await dbContext.MapBookmarks.FindAsync(bookmark.Id);
            bookmarkInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteBookmarkAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(DeleteBookmarkAsync_WithInvalidId_ShouldReturnFalse));
            var service = new BookmarkService(dbContext, GetLogger());
            var userId = Guid.NewGuid();

            // Act
            var result = await service.DeleteBookmarkAsync(Guid.NewGuid(), userId);

            // Assert
            result.Should().BeFalse();
        }
    }
}