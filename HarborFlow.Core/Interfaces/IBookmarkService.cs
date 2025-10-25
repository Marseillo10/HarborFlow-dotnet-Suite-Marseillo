using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlow.Core.Models;

namespace HarborFlow.Core.Interfaces
{
    public interface IBookmarkService
    {
        Task<IEnumerable<MapBookmark>> GetBookmarksForUserAsync(Guid userId);
        Task<MapBookmark> AddBookmarkAsync(MapBookmark bookmark);
        Task<bool> DeleteBookmarkAsync(Guid bookmarkId, Guid userId);
    }
}
