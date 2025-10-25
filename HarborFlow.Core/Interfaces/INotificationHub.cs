using System;
using HarborFlow.Core.Models;

namespace HarborFlow.Core.Interfaces
{
    public interface INotificationHub
    {
        event Action<string, NotificationType> NotificationReceived;
        void SendNotification(string message, NotificationType type);
    }
}
