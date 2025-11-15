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
using MudBlazor.Services;
using HarborFlowSuite.Abstractions.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("HarborFlowSuite.ServerAPI", client => client.BaseAddress = new Uri("http://localhost:5170"))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("HarborFlowSuite.ServerAPI"));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IClientAuthService, AuthService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<IClientServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddScoped<IClientUserProfileService, UserProfileService>();
builder.Services.AddSingleton<MediaQuery>();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateProvider, FirebaseAuthStateProvider>();
builder.Services.AddSingleton<SidebarService>();
builder.Services.AddBlazoredToast();
builder.Services.AddMudServices();

builder.Services.AddSingleton(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var hubConnection = new HubConnectionBuilder()
        .WithUrl("http://localhost:5170/aisHub", options =>
        {
            options.AccessTokenProvider = async () =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<IClientAuthService>();
                    return await authService.GetCurrentUserToken();
                }
            };
        })
        .WithAutomaticReconnect()
        .Build();
    return hubConnection;
});

builder.Services.AddScoped<IVesselPositionSignalRService, VesselPositionSignalRService>(sp =>
{
    var hubConnection = sp.GetRequiredService<HubConnection>();
    var logger = sp.GetRequiredService<ILogger<VesselPositionSignalRService>>();
    return new VesselPositionSignalRService(hubConnection, logger);
});


// Set log level to Debug for all categories
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

await app.RunAsync();