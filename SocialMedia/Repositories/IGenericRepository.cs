namespace SocialMedia.Repositories
{
  public interface IGenericRepository<T> where T : class
  {
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
  }
}
