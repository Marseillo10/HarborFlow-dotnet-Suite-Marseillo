
using System;

namespace HarborFlow.Wpf.Interfaces
{
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public interface INotificationService
    {
        event Action<string, NotificationType> NotificationRequested;
        void ShowNotification(string message, NotificationType type = NotificationType.Error);
        bool ShowConfirmation(string title, string message);
    }
}
