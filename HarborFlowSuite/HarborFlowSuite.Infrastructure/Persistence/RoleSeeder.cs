using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Infrastructure.Persistence;

public class RoleSeeder
{
    private readonly ApplicationDbContext _context;

    public RoleSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var roles = new List<string>
        {
            UserRole.SystemAdmin,
            UserRole.CompanyAdmin,
            UserRole.PortAuthority,
            UserRole.VesselAgent,
            UserRole.Guest
        };

        foreach (var roleName in roles)
        {
            if (!await _context.Roles.AnyAsync(r => r.Name == roleName))
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    Description = GetDescription(roleName),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Roles.Add(role);
            }
        }

        await _context.SaveChangesAsync();
    }

    private string GetDescription(string roleName)
    {
        return roleName switch
        {
            UserRole.SystemAdmin => "Full system access",
            UserRole.CompanyAdmin => "Manage company users and settings",
            UserRole.PortAuthority => "Manage port operations and vessels",
            UserRole.VesselAgent => "Manage vessel arrivals and service requests",
            UserRole.Guest => "Read-only access",
            _ => "Default role"
        };
    }
}
