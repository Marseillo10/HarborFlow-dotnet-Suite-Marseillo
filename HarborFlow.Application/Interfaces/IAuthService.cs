using HarborFlow.Core.Models;
using System.Threading.Tasks;

namespace HarborFlow.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password, string email, string fullName);
        Task<bool> UserExistsAsync(string username);
    }
}
