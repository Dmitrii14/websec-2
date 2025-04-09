using Server.Api.Game;
using Server.Api.Services;
using Server.Domain.Entities;
using Server.Domain.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", builder =>
    {
        builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<IGameRepository<Player>, GameRepository>();
builder.Services.AddSingleton<IGameService<Player>, GameService>();
builder.Services.AddSingleton<GameRoom>();
var app = builder.Build();

app.UseCors("CORS");

app.MapHub<GameHub>("/starship");

app.Run();
