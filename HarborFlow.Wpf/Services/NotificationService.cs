
using HarborFlow.Wpf.Interfaces;
using System;

namespace HarborFlow.Wpf.Services
{
    public class NotificationService : INotificationService
    {
        public event Action<string, NotificationType>? NotificationRequested;

        public void ShowNotification(string message, NotificationType type = NotificationType.Error)
        {
            NotificationRequested?.Invoke(message, type);
        }
    }
}
