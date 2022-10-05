using Microsoft.AspNetCore.Identity;

namespace SignalR.Models;

public class MyUser : IdentityUser<long>
{
    public long JWTVersion { get; set; }
}