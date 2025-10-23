
using HarborFlow.Wpf.Interfaces;
using System;
using System.IO;
using System.Text.Json;

namespace HarborFlow.Wpf.Services
{
    public class AppSettingsModel
    {
        public ThemeType Theme { get; set; } = ThemeType.Light;
    }

    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettingsModel _settings;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolderPath = Path.Combine(appDataPath, "HarborFlow");
            Directory.CreateDirectory(appFolderPath);
            _settingsFilePath = Path.Combine(appFolderPath, "usersettings.json");

            _settings = LoadSettings();
        }

        public ThemeType GetTheme()
        {
            return _settings.Theme;
        }

        public void SetTheme(ThemeType theme)
        {
            _settings.Theme = theme;
            SaveSettings();
        }

        private AppSettingsModel LoadSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return new AppSettingsModel();
            }

            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                return JsonSerializer.Deserialize<AppSettingsModel>(json) ?? new AppSettingsModel();
            }
            catch
            {
                // If file is corrupt, return default settings
                return new AppSettingsModel();
            }
        }

        private void SaveSettings()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch
            {
                // Log error if saving fails
            }
        }
    }
}
