using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        
        var vlaim = this.Context.User.FindFirst(ClaimTypes.Name);
        //string msg = $"用户: {currentUserName},对你说{message}";
        await this.Clients.User(userid.ToString()).SendAsync("PrivateMegReceived", vlaim.Value, message);
    }
    
    
    public Task SendPublicMessage(string message)
    {
        var vlaim = this.Context.User.FindFirst(ClaimTypes.Name);
        var connectionId = this.Context.ConnectionId;
        string msg = $"{DateTime.Now},用户 {vlaim.Value}说： {message}";
        return Clients.All.SendAsync("PublicMsgReceived", msg);
    }
}