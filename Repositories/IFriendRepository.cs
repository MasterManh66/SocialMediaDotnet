using SocialMedia.Models.Entities;

namespace SocialMedia.Repositories
{
  public interface IFriendRepository : IGenericRepository<Friend>
  {
    Task<List<Friend>> GetFriendsByUserId(int userId);
    Task<List<Friend>> SearchFriendByKey(string keyword);
    Task<int> CountFriendByUserId(int userId, DateTime startDate, DateTime endDate);
  }
}
