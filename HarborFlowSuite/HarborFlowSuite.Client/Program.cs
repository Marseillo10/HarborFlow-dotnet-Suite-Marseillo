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
using Microsoft.Extensions.Logging; // Add this using directive

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("HarborFlowSuite.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7274"))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("HarborFlowSuite.ServerAPI"));

builder.Services.AddScoped<HarborFlowSuite.Client.Services.IAuthService, HarborFlowSuite.Client.Services.AuthService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<HarborFlowSuite.Client.Services.IServiceRequestService, HarborFlowSuite.Client.Services.ServiceRequestService>();
builder.Services.AddScoped<IVesselPositionSignalRService, VesselPositionSignalRService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, FirebaseAuthenticationStateProvider>();
builder.Services.AddBlazoredToast();

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
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

await app.RunAsync();
