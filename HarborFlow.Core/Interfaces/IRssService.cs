using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlow.Core.Models;

namespace HarborFlow.Core.Interfaces
{
    public interface IRssService
    {
        Task<IEnumerable<NewsArticle>> FetchNewsAsync(string feedUrl);
    }
}