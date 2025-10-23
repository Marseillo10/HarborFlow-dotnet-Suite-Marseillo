using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using HarborFlow.Application.Interfaces;
using HarborFlow.Infrastructure.Services;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HarborFlow.Wpf.Views;
using HarborFlow.Wpf.ViewModels;
using HarborFlow.Wpf.Services;
using System;

using HarborFlow.Wpf.Interfaces;

using HarborFlow.Wpf.Validators;

namespace HarborFlow.Wpf
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IHost? AppHost { get; private set; }
        public static ThemeType Theme { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MapViewModel>();
                    services.AddSingleton<MapView>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<DashboardView>();
                    services.AddTransient<VesselManagementViewModel>();
                    services.AddTransient<VesselManagementView>();
                    services.AddTransient<VesselEditorViewModel>();
                    services.AddTransient<VesselEditorView>();
                    services.AddTransient<ServiceRequestViewModel>();
                    services.AddTransient<ServiceRequestView>();

                    services.AddSingleton<IAuthService, AuthService>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<LoginView>();

                    services.AddTransient<RegisterViewModel>();
                    services.AddTransient<RegisterView>();
                    services.AddTransient<RegisterViewModelValidator>();

                    services.AddSingleton<SessionContext>();
                    services.AddSingleton<INotificationService, NotificationService>();
                    services.AddSingleton<IWindowManager, WindowManager>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IFileService, FileService>();

                    services.AddScoped<IPortServiceManager, PortServiceManager>();
                    services.AddSingleton<IVesselTrackingService, VesselTrackingService>();

                    services.AddDbContext<HarborFlowDbContext>(options =>
                        options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();

            // Set the initial theme from settings
            var settingsService = AppHost.Services.GetRequiredService<ISettingsService>();
            SetTheme(settingsService.GetTheme());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var windowManager = AppHost.Services.GetRequiredService<IWindowManager>();
            windowManager.ShowLoginWindow();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

        public void SetTheme(ThemeType theme)
        {
            Theme = theme;
            var themeName = theme == ThemeType.Dark ? "DarkTheme" : "LightTheme";
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"Themes/{themeName}.xaml", UriKind.Relative) });
        }
    }
}
