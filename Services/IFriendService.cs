using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.Friend;

namespace SocialMedia.Services
{
  public interface IFriendService
  {
    Task<ApiResponse<FriendDto>> SendFriendRequest(int ReceiverId);
    Task<ApiResponse<List<FriendDto>>> GetFriendsRequest();
    Task<ApiResponse<List<FriendDto>>> GetFriendsReceiver();
    Task<ApiResponse<FriendDto>> AcceptedFriend(int RequestId);
    Task<ApiResponse<List<FriendDto>>> FriendOfUser();
    Task<ApiResponse<string>> DeleteFriend(int FriendId);
    Task<ApiResponse<List<FriendDto>>> SearchFriend(string UserName);
  }
}
