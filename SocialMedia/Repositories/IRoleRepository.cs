using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IRoleRepository : IGenericRepository<Role>
  {
    Task<Role?> GetRoleByNameAsync(string roleName);
  }
}
