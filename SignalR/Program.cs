using SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSignalR();
builder.Services.AddSignalR().AddStackExchangeRedis("127.0.0.1", options =>
{
    options.Configuration.ChannelPrefix = "SignalR_Test";
});
string[] urls = new[] {"http://localhost:5173"};
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder.WithOrigins(urls)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHub<MyHub>("/MyHub");

app.MapControllers();

app.Run();