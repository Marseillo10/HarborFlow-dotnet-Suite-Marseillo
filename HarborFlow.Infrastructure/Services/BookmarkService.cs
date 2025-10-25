using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Infrastructure.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly HarborFlowDbContext _context;
        private readonly ILogger<BookmarkService> _logger;

        public BookmarkService(HarborFlowDbContext context, ILogger<BookmarkService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MapBookmark> AddBookmarkAsync(MapBookmark bookmark)
        {
            if (bookmark == null)
                throw new ArgumentNullException(nameof(bookmark));

            try
            {
                bookmark.Id = Guid.NewGuid();
                bookmark.CreatedAt = DateTime.UtcNow;
                await _context.MapBookmarks.AddAsync(bookmark);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Bookmark {BookmarkName} created for user {UserId}", bookmark.Name, bookmark.UserId);
                return bookmark;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add bookmark for user {UserId}", bookmark.UserId);
                throw;
            }
        }

        public async Task<bool> DeleteBookmarkAsync(Guid bookmarkId, Guid userId)
        {
            try
            {
                var bookmark = await _context.MapBookmarks.FirstOrDefaultAsync(b => b.Id == bookmarkId && b.UserId == userId);
                if (bookmark == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent or unauthorized bookmark {BookmarkId} for user {UserId}", bookmarkId, userId);
                    return false;
                }

                _context.MapBookmarks.Remove(bookmark);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Bookmark {BookmarkId} deleted for user {UserId}", bookmarkId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete bookmark {BookmarkId} for user {UserId}", bookmarkId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<MapBookmark>> GetBookmarksForUserAsync(Guid userId)
        {
            try
            {
                return await _context.MapBookmarks
                    .Where(b => b.UserId == userId)
                    .OrderBy(b => b.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve bookmarks for user {UserId}", userId);
                return Enumerable.Empty<MapBookmark>();
            }
        }
    }
}
