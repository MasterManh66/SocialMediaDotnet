using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface ILikeRepository
  {
    Task<Like?> GetLikeById(int id);
    Task<List<Like>> GetLikesByPostId(int postId);
    Task<List<Like>> GetLikesByUserId(int userId);
    Task<Like?> GetLikeByUserIdAndPostId(int userId, int postId);
    Task<Like?> CreateLike(Like like);
    Task<Like?> DeleteLike(int id);
    Task<int> CountLikeByUserId(int userId, DateTime startDate, DateTime endDate);
  }
}
