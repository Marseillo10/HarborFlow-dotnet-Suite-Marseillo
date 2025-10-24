using System;
using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface ICachingService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null);
        Task InvalidateAsync(string key);
    }
}
