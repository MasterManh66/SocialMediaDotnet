using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IUserRepository
  {
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
    Task UpdateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
  }
}
