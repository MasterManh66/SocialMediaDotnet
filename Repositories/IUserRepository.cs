using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IUserRepository
  {
    Task<User?> GetUserById(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> AddUserAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
  }
}
