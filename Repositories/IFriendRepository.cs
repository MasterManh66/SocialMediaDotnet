using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IFriendRepository
  {
    Task<Friend?> GetFriendById(int id);
    Task<List<Friend>> GetFriendsByUserId(int userId);
    Task<Friend?> CreateFriend(Friend friend);
    Task<Friend?> UpdateFriend(Friend friend);
    Task<Friend?> DeleteFriend(int id);
    Task<List<Friend>> SearchFriendByKey(string keyword);
  }
}
