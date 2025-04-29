using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlPostRepository : SqlGenericRepository<Post>, IPostRepository
  {
    public SqlPostRepository(AppDbContext context) : base(context) {}

    public async Task<Post?> GetPostById(int id)
    {
      return await _context.Posts
        .Include(p => p.User)
        .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<List<Post>> GetPostsByUserId(int userId)
    {
      return await _context.Posts
        .Where(p => p.UserId == userId)
        .Include(p => p.User)
        .ToListAsync();
    }
    public async Task<List<Post>> GetPostsByUserIds(List<int> userIds)
    {
      return await _context.Posts
          .Include(p => p.User)
          .Where(p => userIds.Contains(p.UserId))
          .ToListAsync();
    }
    public async Task<List<Post>> SearchPostByKey(string keyWord)
    {
      return await _context.Posts
        .Where(p =>
          EF.Functions.Like(p.Title, $"%{keyWord.ToLower()}%") ||
          EF.Functions.Like(p.Content, $"%{keyWord.ToLower()}%"))
        .Include(p => p.User)
        .ToListAsync();
    }
    public async Task<int> CountPostsByUserId(int userId, DateTime startDate, DateTime endDate)
    {
      return await _context.Posts
        .Where(p => p.UserId == userId && p.CreatedAt >=  startDate &&  p.CreatedAt <= endDate)
        .CountAsync();
    }
  }
}
