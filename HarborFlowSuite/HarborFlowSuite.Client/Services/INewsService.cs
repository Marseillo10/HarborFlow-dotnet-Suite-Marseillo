using HarborFlowSuite.Core.DTOs;

namespace HarborFlowSuite.Client.Services
{
    public interface INewsService
    {
        Task<List<NewsItemDto>> GetNews();
    }
}
