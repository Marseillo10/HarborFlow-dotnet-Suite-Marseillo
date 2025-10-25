using System;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;

namespace HarborFlow.Application.Services
{
    // This is a singleton service that acts as a central event bus for notifications.
    public class NotificationHub : INotificationHub
    {
        public event Action<string, NotificationType>? NotificationReceived;

        public void SendNotification(string message, NotificationType type)
        {
            NotificationReceived?.Invoke(message, type);
        }
    }
}
