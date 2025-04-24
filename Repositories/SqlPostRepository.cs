using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlPostRepository : IPostRepository
  {
    private readonly AppDbContext _context;
    public SqlPostRepository(AppDbContext context) 
    {
      _context = context;
    }

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
    public async Task<Post?> CreatePost(Post post)
    {
      await _context.Posts.AddAsync(post);
      await _context.SaveChangesAsync();
      return post;
    }
    public async Task<Post?> UpdatePost(Post post)
    {
      _context.Posts.Update(post);
      await _context.SaveChangesAsync();
      return post;
    }
    public async Task<Post?> DeletePost(int id)
    {
      var post = await GetPostById(id);
      if (post != null)
      {
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
      }
      return post;
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
