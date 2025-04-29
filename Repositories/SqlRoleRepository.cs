using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlRoleRepository : SqlGenericRepository<Role>, IRoleRepository
  {
    public SqlRoleRepository(AppDbContext context) : base(context) {}

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
      return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
    }
  }
}
