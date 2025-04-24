using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlRoleRepository : IRoleRepository
  {
    private readonly AppDbContext _context;

    public SqlRoleRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
      return await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
    }

    public async Task AddRoleAsync(Role role)
    {
      await _context.Roles.AddAsync(role);
    }

    public async Task SaveChangesAsync()
    {
      await _context.SaveChangesAsync();
    }
  }
}
