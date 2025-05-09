using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;

namespace SocialMedia.Repositories
{
  public class SqlGenericRepository<T> : IGenericRepository<T> where T : class
  {
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    public SqlGenericRepository(AppDbContext context) 
    { 
      _context = context;
      _dbSet = context.Set<T>();
    }
    public async Task AddAsync(T entity)
    {
      await _dbSet.AddAsync(entity);
      await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(T entity)
    {
      _dbSet.Update(entity);
      await _context.SaveChangesAsync();
    }
    public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
    public async Task DeleteAsync(object id)
    {
      var entity = await GetByIdAsync(id);
      if (entity != null)
      {
        _dbSet.Remove(entity);
      }
    }
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();
  }
}
