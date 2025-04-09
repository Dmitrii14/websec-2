using Server.Domain.Entities;

namespace Server.Domain.Repository;

public class GameRepository:IGameRepository<Player>
{
    private readonly List<Player> _players = [];

    public List<Player> GetAll() => _players;

    public Player? GetByIdAndUsername(string id, string username) =>
        GetAll().FirstOrDefault(p => p.Id == id && p.Username == username);


    public void Add(Player p)
    {
        _players.Add(p);
    }

    public List<Player> GetTopPlayers()
    {
        return [.. GetAll().OrderByDescending(p => p.Rating).Take(10)];
    }

    public void IncrementRating(string id, string username)
    {
        var player = GetByIdAndUsername(id, username);
        if (player != null)
        {
            player.Rating += 1;
        }
    }

    public int GetRatingById(string id, string username)
    {
        var player = GetByIdAndUsername(id, username);
        if (player != null)
        {
            return player.Rating;
        }
        return 0;
    }
}
