using Server.Domain.Entities;

namespace Server.Api.Services;

public interface IGameService<TEntity>
{
    public List<TEntity> GetPlayers();
    public void AddPlayer(TEntity entity);
    public List<Player> GetTopPlayers();
    public void IncrementRating(string id, string username);
    public int GetRatingById(string id, string username);
}
