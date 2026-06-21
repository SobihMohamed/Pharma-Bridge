using Microsoft.AspNetCore.SignalR;

namespace PharmaBridge.Web.Hubs
{
    public class NotificationHub : Hub
    {
        // why it is empty?
        // The NotificationHub class is intentionally left empty
        // because it serves as a SignalR hub for real-time communication between the server and clients.
        // it open the connection and allows to send the notifications to the clients in one way or 
        // send notifications after client make action in website the system send to admin in real time 
        // but it does not need to define any methods for receiving messages from clients in this case.

    }
}
