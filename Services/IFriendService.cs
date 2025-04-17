using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface IFriendService
  {
    Task<ApiResponse<FriendResponse>> SendFriendRequest(int ReceiverId);
    Task<ApiResponse<List<FriendResponse>>> GetFriendsRequest();
    Task<ApiResponse<List<FriendResponse>>> GetFriendsReceiver();
    Task<ApiResponse<FriendResponse>> AcceptedFriend(int RequestId);
    Task<ApiResponse<List<FriendResponse>>> FriendOfUser();
    Task<ApiResponse<string>> DeleteFriend(int FriendId);
    Task<ApiResponse<List<FriendResponse>>> SearchFriend(string UserName);
  }
}
