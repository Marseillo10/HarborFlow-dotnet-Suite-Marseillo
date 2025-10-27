
using HarborFlow.Core.Interfaces;
using HarborFlow.Application.Services;
using HarborFlow.Infrastructure;
using HarborFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure DbContext
builder.Services.AddDbContext<HarborFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("HarborFlowDb")));

builder.Services.AddHttpClient();

// Register application services
builder.Services.AddScoped<IVesselTrackingService, VesselTrackingService>();
builder.Services.AddScoped<IPortServiceManager, PortServiceManager>();
builder.Services.AddScoped<IAisStreamService, AisStreamService>();
builder.Services.AddScoped<ISynchronizationService, SynchronizationService>();
builder.Services.AddScoped<INotificationHub, NotificationHub>();
builder.Services.AddScoped<IRssService, RssService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<SessionContext>();
builder.Services.AddAuthentication("Cookies").AddCookie();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public partial class Program { }
