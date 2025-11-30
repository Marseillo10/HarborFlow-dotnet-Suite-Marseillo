using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(Guid? companyId = null);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task UpdateUserRoleAsync(Guid userId, Guid roleId, string currentFirebaseUid, Guid? companyId = null);
        Task DeleteUserAsync(Guid userId);
        Task DeleteUserByFirebaseIdAsync(string firebaseUid);
    }
}
