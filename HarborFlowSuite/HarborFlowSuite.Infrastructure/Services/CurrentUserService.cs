using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HarborFlowSuite.Infrastructure.Services
{
    public interface ICurrentUserService
    {
        Guid? CompanyId { get; }
        string? UserId { get; }
        string? Role { get; }
        bool IsSystemAdmin { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private Guid? _cachedCompanyId;
        private bool _companyIdResolved;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public Guid? CompanyId
        {
            get
            {
                if (_companyIdResolved) return _cachedCompanyId;

                // 1. Try to get from Claim
                var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("companyId")?.Value;
                if (Guid.TryParse(companyIdClaim, out var companyId))
                {
                    _cachedCompanyId = companyId;
                    _companyIdResolved = true;
                    return _cachedCompanyId;
                }

                // 2. Fallback: Get from Database (Raw SQL to avoid EF Core recursion)
                var userId = UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        var connectionString = _configuration.GetConnectionString("DefaultConnection");
                        using var connection = new Npgsql.NpgsqlConnection(connectionString);
                        connection.Open();

                        using var command = new Npgsql.NpgsqlCommand("SELECT \"CompanyId\" FROM \"Users\" WHERE \"FirebaseUid\" = @UserId", connection);
                        command.Parameters.AddWithValue("@UserId", userId);

                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            if (Guid.TryParse(result.ToString(), out var dbCompanyId))
                            {
                                _cachedCompanyId = dbCompanyId;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CurrentUserService] Error fetching CompanyId from DB: {ex.Message}");
                    }
                }

                _companyIdResolved = true;
                return _cachedCompanyId;
            }
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Role
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;

                if (!string.IsNullOrEmpty(role)) return role;

                // Fallback: Get from Database (Raw SQL to avoid EF Core recursion)
                var userId = UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        var connectionString = _configuration.GetConnectionString("DefaultConnection");
                        using var connection = new Npgsql.NpgsqlConnection(connectionString);
                        connection.Open();

                        // Join Users and Roles to get the Role Name
                        using var command = new Npgsql.NpgsqlCommand(
                            "SELECT r.\"Name\" FROM \"Users\" u JOIN \"Roles\" r ON u.\"RoleId\" = r.\"Id\" WHERE u.\"FirebaseUid\" = @UserId",
                            connection);
                        command.Parameters.AddWithValue("@UserId", userId);

                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            role = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CurrentUserService] Error fetching Role from DB: {ex.Message}");
                    }
                }

                // Debug logging - only if we have a context (user request) but no role
                if (string.IsNullOrEmpty(role) && _httpContextAccessor.HttpContext != null)
                {
                    Console.WriteLine($"[CurrentUserService] Role is null/empty. Claims: {string.Join(", ", _httpContextAccessor.HttpContext?.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>())}");
                }
                return role;
            }
        }

        public bool IsSystemAdmin
        {
            get
            {
                var isAdmin = Role == "SystemAdmin";
                if (!isAdmin)
                {
                    // Console.WriteLine($"[CurrentUserService] IsSystemAdmin is false. Role resolved as: '{Role}'");
                }
                return isAdmin;
            }
        }
    }
}
