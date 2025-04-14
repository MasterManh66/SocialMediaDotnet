using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task AddUserAsync(User user)
    {
      await _context.Users.AddAsync(user);
    }
    public async Task UpdateAsync(User user)
    {
      _context.Users.Update(user);
    }
    public async Task SaveChangesAsync()
    {
      await _context.SaveChangesAsync();
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
  }
}
