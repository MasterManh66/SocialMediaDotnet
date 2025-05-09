using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface ICommentRepository : IGenericRepository<Comment>
  {
    Task<Comment?> GetCommentById(int id);
    Task<List<Comment>> GetCommentsByPostId(int postId);
    Task<List<Comment>> GetCommentsByUserId(int userId);
    Task<Comment?> GetCommentByUserIdAndPostId(int userId, int postId);
    Task<Comment?> GetCommentByIdAndUserId(int id, int userId);
    Task<int> CountCommentsByUserId(int userId, DateTime startDate, DateTime endDate);
  }
}
