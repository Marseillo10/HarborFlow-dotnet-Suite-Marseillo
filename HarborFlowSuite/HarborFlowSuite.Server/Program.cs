using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Infrastructure.Services;
using HarborFlowSuite.Server.Hubs;
using HarborFlowSuite.Server.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()

    .AddJsonOptions(options =>
    {
        // options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HarborFlow API", Version = "v1" });
    // Configure JWT support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddSignalR();
builder.Services.AddHostedService<AisDataService>();

// Configure PostgreSQL and ApplicationDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddNpgsql<ApplicationDbContext>(connectionString);

// Register services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<HarborFlowSuite.Core.Services.IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IUserService, UserService>();

// Configure GFW API client
builder.Services.AddHttpClient<IGfwMetadataService, GfwMetadataService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["GfwApiBaseUrl"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["GfwApiKey"]}");
});

// Configure Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-adminsdk.json"),
});

// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{builder.Configuration["Firebase:ProjectId"]}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{builder.Configuration["Firebase:ProjectId"]}",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Firebase:ProjectId"],
            ValidateLifetime = true,
            RoleClaimType = "role"
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Helper method to get all permission constants
    var permissions = typeof(HarborFlowSuite.Shared.Constants.Permissions)
        .GetNestedTypes()
        .SelectMany(t => t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy))
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
        .Select(fi => fi.GetRawConstantValue()?.ToString())
        .Where(p => p != null)
        .Cast<string>()
        .ToList();

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireAssertion(context =>
            {
                // Check for standard Role claim first (mapped by TokenValidationParameters), then fallback to "role"
                var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                               ?? context.User.FindFirst("role")?.Value;

                // Temporary logging to debug authorization issues
                Console.WriteLine($"[AuthDebug] User: {context.User.Identity?.Name}, Role Claim: {userRole}, Required Permission: {permission}");

                if (string.IsNullOrEmpty(userRole))
                {
                    Console.WriteLine($"[AuthDebug] Access Denied. No role claim found for user '{context.User.Identity?.Name}'. Claims available: {string.Join(", ", context.User.Claims.Select(c => c.Type))}");
                    return false;
                }

                var rolePermissions = HarborFlowSuite.Shared.Security.RolePermissions.GetPermissionsForRole(userRole);
                var hasPermission = rolePermissions.Contains(permission);

                if (!hasPermission)
                {
                    Console.WriteLine($"[AuthDebug] Access Denied. Role: '{userRole}' does not have permission: '{permission}'");
                }

                return hasPermission;
            }));
    }
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:7163", "http://localhost:5205") // Adjust ports if needed
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HarborFlow API v1"));
    // Apply migrations on startup in development environment
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        var portSeeder = new PortSeeder(dbContext);
        await portSeeder.SeedAsync();

        var roleSeeder = new RoleSeeder(dbContext);
        await roleSeeder.SeedAsync();
    }
}

// app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AisHub>("/aisHub");

app.Run();

