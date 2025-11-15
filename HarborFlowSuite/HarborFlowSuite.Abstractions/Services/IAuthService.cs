using HarborFlowSuite.Core.Models;
using System.Threading.Tasks;

namespace HarborFlowSuite.Abstractions.Services
{
    public interface IAuthService
    {
        Task<User> RegisterUserAsync(User user);
    }
}
