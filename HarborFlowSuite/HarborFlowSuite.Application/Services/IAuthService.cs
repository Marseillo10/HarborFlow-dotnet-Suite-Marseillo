using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;

namespace HarborFlowSuite.Application.Services;

public interface IAuthService
{
    Task<User> RegisterUserAsync(RegisterUserDto registerUserDto);
}
