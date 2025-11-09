using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Application.Services;

public interface IAuthService
{
    Task<User> RegisterUserAsync(User user);
}
