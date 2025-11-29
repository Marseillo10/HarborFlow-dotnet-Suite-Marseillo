using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task UpdateUserRoleAsync(Guid userId, Guid roleId);
        Task DeleteUserAsync(Guid userId);
        Task DeleteUserByFirebaseIdAsync(string firebaseUid);
    }
}
