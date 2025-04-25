using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface ILikeRepository : IGenericRepository<Like>
  {
    Task<Like?> GetLikeById(int id);
    Task<List<Like>> GetLikesByPostId(int postId);
    Task<List<Like>> GetLikesByUserId(int userId);
    Task<Like?> GetLikeByUserIdAndPostId(int userId, int postId);
    Task<int> CountLikeByUserId(int userId, DateTime startDate, DateTime endDate);
  }
}
