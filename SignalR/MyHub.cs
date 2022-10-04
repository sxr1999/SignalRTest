using Microsoft.AspNetCore.SignalR;

namespace SignalR;

public class MyHub : Hub
{
    public Task SendPublicMessage(string message)
    {
        var connectionId = this.Context.ConnectionId;
        string msg = $"{connectionId} {DateTime.Now}:{message}";
        return Clients.All.SendAsync("PublicMsgReceived", msg);
    }
}