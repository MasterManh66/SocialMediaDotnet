using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface ICommentRepository
  {
    Task<Comment?> GetCommentById(int id);
    Task<List<Comment>> GetCommentsByPostId(int postId);
    Task<List<Comment>> GetCommentsByUserId(int userId);
    Task<Comment?> GetCommentByUserIdAndPostId(int userId, int postId);
    Task<Comment?> CreateComment(Comment comment);
    Task<Comment?> DeleteComment(int id);
    Task<Comment?> UpdateComment(Comment comment);
    Task<Comment?> GetCommentByIdAndUserId(int id, int userId);
    Task<int> CountCommentsByUserId(int userId, DateTime startDate, DateTime endDate);
  }
}
