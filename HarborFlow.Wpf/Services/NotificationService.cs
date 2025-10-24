
using HarborFlow.Wpf.Interfaces;
using System;
using System.Windows;

namespace HarborFlow.Wpf.Services
{
    public class NotificationService : INotificationService
    {
        public event Action<string, NotificationType>? NotificationRequested;

        public void ShowNotification(string message, NotificationType type = NotificationType.Error)
        {
            NotificationRequested?.Invoke(message, type);
        }

        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }
    }
}
