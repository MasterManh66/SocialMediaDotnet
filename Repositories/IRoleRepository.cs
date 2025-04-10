using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IRoleRepository
  {
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task AddRoleAsync(Role role);
    Task SaveChangesAsync();
  }
}
