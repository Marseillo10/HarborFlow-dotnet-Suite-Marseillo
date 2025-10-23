
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.ViewModels;
using HarborFlow.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace HarborFlow.Wpf.Services
{
    public class WindowManager : IWindowManager
    {
        private readonly IServiceProvider _serviceProvider;
        private Window? _currentWindow;

        public WindowManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowLoginWindow()
        {
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            _currentWindow?.Close();
            _currentWindow = loginView;
            loginView.Show();
        }

        public void ShowMainWindow()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            _currentWindow?.Close();
            _currentWindow = mainWindow;
            mainWindow.Show();
        }

        public bool? ShowVesselEditorDialog(Vessel vessel)
        {
            var viewModel = new VesselEditorViewModel(vessel);
            var editorView = new VesselEditorView(viewModel)
            {
                Owner = _currentWindow
            };
            return editorView.ShowDialog();
        }

        public bool? ShowServiceRequestEditorDialog(ServiceRequest serviceRequest)
        {
            var viewModel = new ServiceRequestEditorViewModel(serviceRequest);
            var editorView = new ServiceRequestEditorView(viewModel)
            {
                Owner = _currentWindow
            };
            return editorView.ShowDialog();
        }
    }
}
