using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlLikeRepository : ILikeRepository
  {
    private readonly AppDbContext _context;
    public SqlLikeRepository(AppDbContext context) 
    {
      _context = context;
    }
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
    public async Task<Like?> CreateLike(Like like)
    {
      await _context.Likes.AddAsync(like);
      await _context.SaveChangesAsync();
      return like;
    }
    public async Task<Like?> DeleteLike(int id)
    {
      var like = await GetLikeById(id);
      if (like != null)
      {
        _context.Likes.Remove(like);
        await _context.SaveChangesAsync();
      }
      return like;
    }
    public async Task<int> CountLikeByUserId(int userId, DateTime startDate, DateTime endDate)
    {
      return await _context.Likes
        .Where(l => l.UserId == userId && l.CreatedAt >= startDate && l.CreatedAt <= endDate)
        .CountAsync();
    }
  }
}
