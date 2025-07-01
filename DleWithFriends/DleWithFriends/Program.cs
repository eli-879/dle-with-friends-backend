using DleWithFriends.GameServer;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//    ConnectionMultiplexer.Connect("localhost:7224"));
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:5173");
    });
});

builder.Services.AddSingleton<IGameRoomManager, InMemGameRoomManager>();

var app = builder.Build();
app.UseCors();
app.MapHub<GameHub>("/gamehub");


app.Run();


//var gameServer = new GameServer();
//await gameServer.StartServer();

