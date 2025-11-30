using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HarborFlowSuite.Client;
using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Core.Services;
using HarborFlowSuite.Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("HarborFlowSuite.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7274"))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("HarborFlowSuite.ServerAPI"));

builder.Services.AddScoped<HarborFlowSuite.Client.Services.IAuthService, HarborFlowSuite.Client.Services.AuthService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<HarborFlowSuite.Client.Services.IServiceRequestService, HarborFlowSuite.Client.Services.ServiceRequestService>();
builder.Services.AddScoped<IVesselPositionSignalRService, VesselPositionSignalRService>();
builder.Services.AddScoped<PortService>();
builder.Services.AddScoped<IServiceRequestSignalRService, ServiceRequestSignalRService>();
builder.Services.AddScoped<INewsService, NewsService>();

builder.Services.AddAuthorizationCore(options =>
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
                var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                               ?? context.User.FindFirst("role")?.Value;

                if (string.IsNullOrEmpty(userRole)) return false;

                var rolePermissions = HarborFlowSuite.Shared.Security.RolePermissions.GetPermissionsForRole(userRole);
                return rolePermissions.Contains(permission);
            }));
    }
});

builder.Services.AddScoped<AuthenticationStateProvider, FirebaseAuthenticationStateProvider>();
builder.Services.AddBlazoredToast();
builder.Services.AddMudServices();
builder.Services.AddScoped<IIdleTimeoutService, IdleTimeoutService>();

builder.Services.AddSingleton(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var hubConnection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7274/aisHub", options =>
        {
            options.AccessTokenProvider = async () =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<HarborFlowSuite.Client.Services.IAuthService>();
                    return await authService.GetCurrentUserToken();
                }
            };
        })
        .WithAutomaticReconnect()
        .Build();
    return hubConnection;
});

// Set log level to Debug for all categories
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var app = builder.Build();

await app.RunAsync();
