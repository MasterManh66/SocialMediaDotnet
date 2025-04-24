using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IUserRepository : IGenericRepository<User>
  {
    Task<User?> GetUserById(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetByEmailAsync(string email);
  }
}
