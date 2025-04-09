using Server.Domain.Entities;
using Server.Domain.Repository;

namespace Server.Api.Services;

public class GameService(IGameRepository<Player> repository) : IGameService<Player>
{
    public List<Player> GetPlayers() => repository.GetAll();
    public void AddPlayer(Player player) => repository.Add(player);
    public List<Player> GetTopPlayers() => repository.GetTopPlayers();
    public void IncrementRating(string id, string username) => repository.IncrementRating(id, username);
    public int GetRatingById(string id, string username) => repository.GetRatingById(id, username);
}
