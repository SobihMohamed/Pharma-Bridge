using PharmaBridge.Shared.Dto_s.Notificaiton;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.Notification
{
    // we will use the strategy design pattern to implement different notification strategies (e.g., email, SMS, push notifications)
    public interface INotificationStrategy
    {
        NotificationType Type { get; }
        Task DeliverAsync(MessageDto message);
    }
}
