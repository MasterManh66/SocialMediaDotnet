using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IPostRepository : IGenericRepository<Post>
  {
    Task<Post?> GetPostById(int id);
    Task<List<Post>> GetPostsByUserId(int userId);
    Task<List<Post>> GetPostsByUserIds(List<int> userIds);
    Task<List<Post>> SearchPostByKey(string Keyword);
    Task<int> CountPostsByUserId(int userId, DateTime startDate,  DateTime endDate);
  }
}
