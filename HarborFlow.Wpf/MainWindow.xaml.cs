using System.Windows;
using System.Windows.Controls;
using HarborFlow.Wpf.Views;
using HarborFlow.Wpf.ViewModels;
using System;

using HarborFlow.Wpf.Interfaces;

using System.Windows.Shapes;
using System.Windows.Media;

using System.Threading.Tasks;

namespace HarborFlow.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly ISettingsService _settingsService;

        public MainWindow(MainWindowViewModel viewModel, ISettingsService settingsService)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _settingsService = settingsService;

            UpdateThemeIcon(App.Theme);
        }

        private void ToggleThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)System.Windows.Application.Current;
            var newTheme = App.Theme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
            app.SetTheme(newTheme);
            _settingsService.SetTheme(newTheme);
            UpdateThemeIcon(newTheme);
        }

        private void UpdateThemeIcon(ThemeType theme)
        {
            var iconKey = theme == ThemeType.Light ? "MoonIcon" : "SunIcon";
            if (ToggleThemeButton.Content is Path path)
            {
                path.Data = (PathGeometry)FindResource(iconKey);
            }
        }
    }
}
