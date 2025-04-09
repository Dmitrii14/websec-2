using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Server.Api.Game;

public class GameHub(GameRoom gameRoom, ILogger<GameHub> logger) : Hub
{
    private readonly GameRoom _gameRoom = gameRoom;
    private readonly ILogger<GameHub> _logger = logger;

    public async Task RegisterPlayer(string name)
    {
        try
        {
            await _gameRoom.RegisterPlayer(Context.ConnectionId, name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации игрока: {Name}", name);
            throw;
        }
    }

    public async Task CheckGame()
    {
        try
        {
            await _gameRoom.CheckGame();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при CheckGame");
            throw;
        }
    }

    public void Move(List<string> directions)
    {
        try
        {
            _gameRoom.MovePlayer(Context.ConnectionId, directions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при Move игрока с ID: {ConnectionId}", Context.ConnectionId);
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            await _gameRoom.RemovePlayer(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отключении игрока с ID: {ConnectionId}", Context.ConnectionId);
            throw;
        }
    }

    public async Task LeaveGame()
    {
        try
        {
            await _gameRoom.RemovePlayer(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выходе игрока с ID: {ConnectionId}", Context.ConnectionId);
            throw;
        }
    }
}
