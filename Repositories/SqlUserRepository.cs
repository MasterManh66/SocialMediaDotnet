using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlUserRepository : SqlGenericRepository<User>, IUserRepository
  {
    public SqlUserRepository(AppDbContext context) : base(context) {}
    public async Task<User?> GetUserById(int userId)
    {
      return await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
  }
}
