using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class CommentRepository : ICommentRepository
  {
    private readonly AppDbContext _context;
    public CommentRepository(AppDbContext context)
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
    public async Task<Comment?> CreateComment(Comment comment)
    {
      await _context.Comments.AddAsync(comment);
      await _context.SaveChangesAsync();
      return comment;
    }
    public async Task<Comment?> DeleteComment(int id)
    {
      var comment = await GetCommentById(id);
      if (comment != null)
      {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
      }
      return comment;
    }
    public async Task<Comment?> UpdateComment(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
      return comment;
    }
    public async Task<Comment?> GetCommentByIdAndUserId(int id, int userId)
    {
      return await _context.Comments
        .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }
  }
}
