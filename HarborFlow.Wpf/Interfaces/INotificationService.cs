
using System;
using HarborFlow.Core.Models;

namespace HarborFlow.Wpf.Interfaces
{

    public interface INotificationService
    {
        event Action<string, NotificationType> NotificationRequested;
                void ShowNotification(string message, NotificationType type = NotificationType.Error);
        bool ShowConfirmation(string title, string message);

    }
}
