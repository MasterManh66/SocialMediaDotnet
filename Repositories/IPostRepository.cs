using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IPostRepository
  {
    Task<Post?> GetPostById(int id);
    Task<List<Post>> GetPostsByUserId(int userId);
    Task<Post?> CreatePost(Post post);
    Task<Post?> UpdatePost(Post post);
    Task<Post?> DeletePost(int id);
    Task<List<Post>> SearchPostByKey(string Keyword);
  }
}
