using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Domain.Enums;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class SqlFriendRepository : SqlGenericRepository<Friend>, IFriendRepository
  {
    private readonly AppDbContext _context;
    public SqlFriendRepository(AppDbContext context) : base(context)
    {
      _context = context;
    }
    public async Task<List<Friend>> GetFriendsByUserId(int userId)
    {
      return await _context.Friends
        .Include(f => f.Receiver)
        .Include(f => f.Requester)
        .Where(f => f.RequesterId == userId || f.ReceiverId == userId)
        .ToListAsync();
    }
    public async Task<List<Friend>> SearchFriendByKey(string keyword)
    {
      return await _context.Friends
        .Include(f => f.Requester)
        .Include(f => f.Receiver)
        .Where(f =>
          EF.Functions.Like((f.Requester != null ? f.Requester.FirstName + " " + f.Requester.LastName : ""), $"%{keyword}%") ||
          EF.Functions.Like((f.Receiver != null ? f.Receiver.FirstName + " " + f.Receiver.LastName : ""), $"%{keyword}%"))
        .ToListAsync();
    }
    public async Task<int> CountFriendByUserId(int userId, DateTime startDate, DateTime endDate)
    {
      return await _context.Friends
        .Where(f => (f.RequesterId == userId || f.ReceiverId == userId) && f.FriendStatus == FriendEnum.Accepted
               && f.CreatedAt >= startDate && f.CreatedAt <= endDate)
        .CountAsync();
    }
  }
}
