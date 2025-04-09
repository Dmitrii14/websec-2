namespace Server.Domain.Repository;

public interface IGameRepository<TEntity>
{
    public void Add(TEntity player);
    public List<TEntity> GetTopPlayers();
    public List<TEntity> GetAll();
    public TEntity? GetByIdAndUsername(string id, string username);
    public int GetRatingById(string id, string username);
    public void IncrementRating(string id, string username);
}
