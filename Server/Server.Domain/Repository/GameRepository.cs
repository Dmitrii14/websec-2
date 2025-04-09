using Server.Domain.Entities;

namespace Server.Domain.Repository;

public class GameRepository:IGameRepository<Player>
{
    private readonly List<Player> _players = [];

    public List<Player> GetAll() => _players;

    public Player? GetById(string id) => GetAll().FirstOrDefault(p => p.Id == id);

    public void Add(Player p)
    {
        var player = GetById(p.Id);
        if (player != null)
        {
            player.Username = p.Username;
            return;
        }
        _players.Add(p);
    }

    public List<Player> GetTopPlayers()
    {
        return [.. GetAll().OrderByDescending(p => p.Rating).Take(10)];
    }

    public void IncrementRating(string id)
    {
        var player = GetById(id);
        if (player != null)
        {
            player.Rating += 1;
        }
    }

    public int GetRatingById(string id)
    {
        var player = GetById(id);
        if (player != null)
        {
            return player.Rating;
        }
        return 0;
    }
}
