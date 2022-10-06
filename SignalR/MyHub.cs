using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR;

[Authorize]
public class MyHub : Hub
{
    private readonly UserManager<MyUser> _userManager;

    public MyHub(UserManager<MyUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task SendPrivateMsg(string toUserName, string message)
    {
        var user =await _userManager.FindByNameAsync(toUserName);
        long userid = user.Id;
        var currentUserName = this.Context.UserIdentifier;
        //string msg = $"用户: {currentUserName},对你说{message}";
        await this.Clients.User(userid.ToString()).SendAsync("PrivateMegReceived", currentUserName, message);
    }
    
    
    public Task SendPublicMessage(string message)
    {
        var vlaim = this.Context.User.FindFirst(ClaimTypes.Name);
        var connectionId = this.Context.ConnectionId;
        string msg = $"{vlaim.Value} {DateTime.Now}:{message}";
        return Clients.All.SendAsync("PublicMsgReceived", msg);
    }
}