using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HarborFlowSuite.Client;
using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.Toast;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("HarborFlowSuite.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7274"))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("HarborFlowSuite.ServerAPI"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, FirebaseAuthenticationStateProvider>();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
