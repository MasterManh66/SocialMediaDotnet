using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlCommentRepository : SqlGenericRepository<Comment>, ICommentRepository
  {
    private readonly AppDbContext _context;
    public SqlCommentRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }
    public async Task<Comment?> GetCommentById(int id)
    {
      return await _context.Comments
        .Include(c => c.User)
        .Include(c => c.Post)
        .FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<List<Comment>> GetCommentsByPostId(int postId)
    {
      return await _context.Comments
        .Where(c => c.PostId == postId)
        .Include(c => c.User)
        .ToListAsync();
    }
    public async Task<List<Comment>> GetCommentsByUserId(int userId)
    {
      return await _context.Comments
        .Where(c => c.UserId == userId)
        .Include(c => c.Post)
        .ToListAsync();
    }
    public async Task<Comment?> GetCommentByUserIdAndPostId(int userId, int postId)
    {
      return await _context.Comments
        .FirstOrDefaultAsync(c => c.UserId == userId && c.PostId == postId);
    }
    public async Task<Comment?> GetCommentByIdAndUserId(int id, int userId)
    {
      return await _context.Comments
        .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }
    public async Task<int> CountCommentsByUserId(int userId, DateTime startDate, DateTime endDate)
    {
      return await _context.Comments
        .Where(c => c.UserId == userId && c.CreatedAt >= startDate && c.CreatedAt <= endDate)
        .CountAsync();
    }
  }
}
