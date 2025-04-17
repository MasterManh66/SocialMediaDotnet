using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public class FriendRepository : IFriendRepository
  {
    private readonly AppDbContext _context;
    public FriendRepository(AppDbContext context)
    {
      _context = context;
    }
    public async Task<Friend?> GetFriendById(int friendId)
    {
      return await _context.Friends.FindAsync(friendId);
    }
    public async Task<List<Friend>> GetFriendsByUserId(int userId)
    {
      return await _context.Friends
        .Include(f => f.Receiver)
        .Include(f => f.Requester)
        .Where(f => f.RequesterId == userId || f.ReceiverId == userId)
        .ToListAsync();
    }
    public async Task<Friend?> CreateFriend(Friend friend)
    {
      await _context.Friends.AddAsync(friend);
      await _context.SaveChangesAsync();
      return friend;
    }
    public async Task<Friend?> UpdateFriend(Friend friend)
    {
      _context.Friends.Update(friend);
      await _context.SaveChangesAsync();
      return friend;
    }
    public async Task<Friend?> DeleteFriend(int friendId)
    {
      var friend = await GetFriendById(friendId);
      if (friend == null)
      {
        return null;
      }
      _context.Friends.Remove(friend);
      await _context.SaveChangesAsync();
      return friend;
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
  }
}
