using System.Security.Claims;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Server.Middleware;

public class UserSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserSyncMiddleware> _logger;

    public UserSyncMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, ILogger<UserSyncMiddleware> logger)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var firebaseUid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            // Firebase token might not have name claim directly, sometimes it's in "name" or "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            var name = context.User.FindFirst(ClaimTypes.Name)?.Value ?? context.User.FindFirst("name")?.Value ?? email;

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Check if user exists in local DB (using raw SQL for speed/avoiding tracking issues if needed, but EF is fine here)
                    var userExists = await dbContext.Users.AnyAsync(u => u.FirebaseUid == firebaseUid);

                    if (!userExists)
                    {
                        _logger.LogInformation("User {FirebaseUid} ({Email}) authenticated but not found in local DB. Syncing...", firebaseUid, email);

                        // Determine Role
                        string roleName = UserRole.Guest;
                        if (email != null && email.EndsWith("@mail.ugm.ac.id", StringComparison.OrdinalIgnoreCase))
                        {
                            roleName = UserRole.SystemAdmin;
                        }

                        // Get or Create Role
                        var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                        if (role == null)
                        {
                            role = new Role
                            {
                                Id = Guid.NewGuid(),
                                Name = roleName,
                                Description = roleName == UserRole.SystemAdmin ? "System Administrator" : "Default Guest Role",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            dbContext.Roles.Add(role);
                            await dbContext.SaveChangesAsync();
                        }

                        // Create User
                        var newUser = new User
                        {
                            Id = Guid.NewGuid(),
                            FirebaseUid = firebaseUid,
                            Email = email ?? "",
                            FullName = name ?? "Unknown",
                            RoleId = role.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        dbContext.Users.Add(newUser);
                        await dbContext.SaveChangesAsync();

                        _logger.LogInformation("User {FirebaseUid} synced to local DB successfully.", firebaseUid);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing user {FirebaseUid} to local DB.", firebaseUid);
                    // We don't throw here to avoid blocking the request if DB is momentarily flaky, 
                    // but downstream services relying on DB user might fail.
                }
            }
        }

        await _next(context);
    }
}
