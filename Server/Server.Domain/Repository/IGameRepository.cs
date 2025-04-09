namespace Server.Domain.Repository;

public interface IGameRepository<TEntity>
{
    public void Add(TEntity player);
    public List<TEntity> GetTopPlayers();
    public List<TEntity> GetAll();
    public TEntity? GetById(string id);
    public int GetRatingById(string id);
    public void IncrementRating(string id);
}
