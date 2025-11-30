using HarborFlowSuite.Core.DTOs;

namespace HarborFlowSuite.Application.Services
{
    public interface INewsService
    {
        Task<List<NewsItemDto>> GetNewsAsync(CancellationToken cancellationToken = default);
    }
}
