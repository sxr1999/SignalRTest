using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignalR.DbContext;

public class MyDbContext : IdentityDbContext<MyUser,MyRole,long>
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
        
    }

    public DbSet<MyUser> MyUsers { get; set; }
    public DbSet<MyRole> MyRoles { get; set; }
}