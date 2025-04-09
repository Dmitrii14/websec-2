using Microsoft.AspNetCore.SignalR;
using Server.Domain.Entities;
using System.Collections.Concurrent;
using Server.Api.Services;

namespace Server.Api.Game;


public class GameRoom
{
    private static readonly ConcurrentDictionary<string, Player> Players = new();
    private static Star Star = new();
    private static readonly double GameLoopInterval = 1000 / 60;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly System.Timers.Timer _gameLoopTimer;
    private readonly IGameService<Player> _service;

    public GameRoom(IHubContext<GameHub> hubContext, IGameService<Player> service)
    {
        _service = service;
        _hubContext = hubContext;
        _gameLoopTimer = new System.Timers.Timer(GameLoopInterval);
        _gameLoopTimer.Elapsed += async (sender, e) => await UpdateGame();
        _gameLoopTimer.Start();
    }

    public async Task CheckGame()
    {
        var topPlayers = _service.GetTopPlayers();
        await _hubContext.Clients.All.SendAsync("StarCollected", Star);
        await _hubContext.Clients.All.SendAsync("TopPlayers", topPlayers);
    }

    public async Task RegisterPlayer(string connectionId, string name)
    {
        if (Players.Count >= 10)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Info", "full");
            return;
        }
        else
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Info", "empty");
        }

        var car = new Ship
        {
            Id = connectionId
        };

        var player = new Player
        {
            Id = connectionId,
            Username = name,
            Car = car
        };

        _service.AddPlayer(player);
        Players[connectionId] = player;

        var topPlayers = _service.GetTopPlayers();
        await _hubContext.Clients.All.SendAsync("TopPlayers", topPlayers);
    }


    public void MovePlayer(string connectionId, List<string> directions)
    {
        if (!Players.TryGetValue(connectionId, out var player)) return;

        var car = player.Car;

        foreach (var direction in directions)
        {
            switch (direction)
            {
                case "left":
                    car.Angle -= car.RotationSpeed;
                    break;
                case "right":
                    car.Angle += car.RotationSpeed;
                    break;
                case "up":
                    car.Velocity += car.Acceleration;
                    break;
                case "down":
                    car.Velocity -= car.Acceleration;
                    break;
            }
        }

        car.Velocity = Math.Clamp(car.Velocity, -car.MaxVelocity, car.MaxVelocity);
    }



    public async Task UpdateGame()
    {
        float friction = 0.01f;

        foreach (var player in Players.Values)
        {
            var car = player.Car;

            if (car.Velocity > 0)
                car.Velocity -= friction;
            else if (car.Velocity < 0)
                car.Velocity += friction;

            if (Math.Abs(car.Velocity) < friction)
                car.Velocity = 0;

            float speedX = (float)(Math.Cos(car.Angle) * car.Velocity);
            float speedY = (float)(Math.Sin(car.Angle) * car.Velocity);

            car.X += speedX;
            car.Y += speedY;

            player.Car.X = Math.Clamp(player.Car.X, 0, 800);
            player.Car.Y = Math.Clamp(player.Car.Y, 0, 500);


            if (car.X < Star.X + Star.Size && car.X + car.Hitbox > Star.X - Star.Size &&
                car.Y < Star.Y + Star.Size && car.Y + car.Hitbox > Star.Y - Star.Size)
            {
                Star = new Star();
                await _hubContext.Clients.All.SendAsync("StarCollected", Star);
                _service.IncrementRating(player.Id, player.Username);
                await _hubContext.Clients.Client(player.Id).SendAsync("ReceiveStars", _service.GetRatingById(player.Id, player.Username));
                await _hubContext.Clients.All.SendAsync("TopPlayers", _service.GetTopPlayers());
            }
        

        foreach (var otherPlayer in Players.Values)
            {
                if (otherPlayer.Id != player.Id)
                {
                    if (player.Car.X < otherPlayer.Car.X + otherPlayer.Car.Hitbox && player.Car.X + player.Car.Hitbox > otherPlayer.Car.X &&
                        player.Car.Y < otherPlayer.Car.Y + otherPlayer.Car.Hitbox && player.Car.Y + player.Car.Hitbox > otherPlayer.Car.Y)
                    {
                        float tempVX = player.Car.SpeedX;
                        float tempVY = player.Car.SpeedY;

                        player.Car.SpeedX = otherPlayer.Car.SpeedX;
                        player.Car.SpeedY = otherPlayer.Car.SpeedY;

                        otherPlayer.Car.SpeedX = tempVX;
                        otherPlayer.Car.SpeedY = tempVY;

                        player.Car.X += player.Car.SpeedX;
                        player.Car.Y += player.Car.SpeedY;

                        otherPlayer.Car.X += otherPlayer.Car.SpeedX;
                        otherPlayer.Car.Y += otherPlayer.Car.SpeedY;

                        float overlapX = (player.Car.Hitbox + otherPlayer.Car.Hitbox) / 2 - Math.Abs(player.Car.X - otherPlayer.Car.X);
                        float overlapY = (player.Car.Hitbox + otherPlayer.Car.Hitbox) / 2 - Math.Abs(player.Car.Y - otherPlayer.Car.Y);

                        if (overlapX > 0 && overlapY > 0)
                        {
                            if (overlapX < overlapY)
                            {
                                player.Car.X += overlapX / 2 * Math.Sign(player.Car.X - otherPlayer.Car.X);
                                otherPlayer.Car.X -= overlapX / 2 * Math.Sign(player.Car.X - otherPlayer.Car.X);
                            }
                            else
                            {
                                player.Car.Y += overlapY / 2 * Math.Sign(player.Car.Y - otherPlayer.Car.Y);
                                otherPlayer.Car.Y -= overlapY / 2 * Math.Sign(player.Car.Y - otherPlayer.Car.Y);
                            }
                        }
                    }
                }
            }
        }
        await _hubContext.Clients.All.SendAsync("GameState", Players.Values);
    }


    public async Task RemovePlayer(string connectionId)
    {
        if (Players.TryRemove(connectionId, out _))
        {
            await _hubContext.Clients.All.SendAsync("PlayerLeft", connectionId);
        }
        if (Players.Count < 10) await _hubContext.Clients.All.SendAsync("Info", "empty");
    }
}

