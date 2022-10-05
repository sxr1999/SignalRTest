using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalR;
using SignalR.DbContext;
using SignalR.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
// builder.Services.AddSignalR().AddStackExchangeRedis("127.0.0.1", options =>
// {
//     options.Configuration.ChannelPrefix = "SignalR_Test";
// });
string[] urls = new[] {"http://localhost:5173"};
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(urls)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    );
});


string mySqlConnectionStr = builder.Configuration.GetConnectionString("MySQL");
builder.Services.AddDbContext<MyDbContext>(opt => opt.UseMySql(mySqlConnectionStr,ServerVersion.AutoDetect(mySqlConnectionStr)));

builder.Services.AddDataProtection();

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        var jwtopt = builder.Configuration.GetSection("JWT").Get<JWTSettings>();
        byte[] keyBytes = Encoding.UTF8.GetBytes(jwtopt.Key);
        var secKey = new SymmetricSecurityKey(keyBytes);
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = secKey
        };
        opt.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                //websocket不支持自定义请求头，所以需要把JWT放入QueryString中传递
                var accessToken = context.Request.Query["access_token"];
                var path = context.Request.Path;
                //如果请求字符串不为空且请求路径是/MyHub，那么context中的token就是accessToken,
                //服务器的OnMessageReceived方法会把querystring中的jwt读出来，然后复制给context.token;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/MyHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

    });



builder.Services.AddIdentity<MyUser,MyRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
    })
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<MyHub>("/MyHub");

app.MapControllers();

app.Run();