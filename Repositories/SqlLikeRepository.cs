using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlLikeRepository : SqlGenericRepository<Like>, ILikeRepository
  {
    public SqlLikeRepository(AppDbContext context) : base(context) {}
    public async Task<Like?> GetLikeById(int id)
    {
      return await _context.Likes
        .Include(l => l.User)
        .Include(l => l.Post)
        .FirstOrDefaultAsync(l => l.Id == id);
    }
    public async Task<List<Like>> GetLikesByPostId(int postId)
    {
      return await _context.Likes
        .Where(l => l.PostId == postId)
        .Include(l => l.User)
        .ToListAsync();
    }
    public async Task<List<Like>> GetLikesByUserId(int userId)
    {
      return await _context.Likes
        .Where(l => l.UserId == userId)
        .Include(l => l.Post)
        .ToListAsync();
    }
    public async Task<Like?> GetLikeByUserIdAndPostId(int userId, int postId)
    {
      return await _context.Likes
        .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
    }
    public async Task<int> CountLikeByUserId(int userId, DateTime startDate, DateTime endDate)
    {
      return await _context.Likes
        .Where(l => l.UserId == userId && l.CreatedAt >= startDate && l.CreatedAt <= endDate)
        .CountAsync();
    }
  }
}
