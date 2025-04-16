using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface IFriendService
  {
    Task<ApiResponse<FriendResponse>> SendFriendRequest(int ReceiverId);
    Task<ApiResponse<List<FriendResponse>>> GetFriendsRequest();
  }
}
