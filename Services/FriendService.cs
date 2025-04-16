using SocialMedia.Models.Entities;
using System.Security.Claims;
using SocialMedia.Repositories;
using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Enums;

namespace SocialMedia.Services
{
  public class FriendService : IFriendService
  {
    private readonly IFriendRepository _friendRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FriendService(IHttpContextAccessor httpContextAccessor, IFriendRepository friendRepository, IUserRepository userRepository)
    {
      _httpContextAccessor = httpContextAccessor;
      _friendRepository = friendRepository;
      _userRepository = userRepository;
    }
    public string? GetCurrentUserEmail()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
    public Task<User?> GetUserByEmailAsync(string email)
    {
      return _userRepository.GetByEmailAsync(email);
    }
    public async Task<ApiResponse<FriendResponse>> SendFriendRequest(int ReceiverId)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<FriendResponse>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      var fullName = user != null ? $"{user.FirstName} {user.LastName}" : "Anonymous";
      if (user == null)
      {
        return new ApiResponse<FriendResponse>(404, "Người dùng không tồn tại!", null);
      }
      //check request
      if (ReceiverId <= 0)
      {
        return new ApiResponse<FriendResponse>(400, "ID người nhận không hợp lệ!", null);
      }
      if (ReceiverId == user.Id)
      {
        return new ApiResponse<FriendResponse>(400, "Bạn không thể gửi lời mời kết bạn cho chính mình!", null);
      }
      //check receiver
      var receiver = await _userRepository.GetUserById(ReceiverId);
      if (receiver == null)
      {
        return new ApiResponse<FriendResponse>(404, $"Người dùng {ReceiverId} không tồn tại!", null);
      }
      //check friend
      var friend = await _friendRepository.GetFriendsByUserId(user.Id);
      if (friend != null && friend.Any(f => f.RequesterId == user.Id && f.ReceiverId == ReceiverId))
      {
        return new ApiResponse<FriendResponse>(400, $"Bạn đã gửi lời mời kết bạn cho người dùng {ReceiverId} trước đó!", null);
      }
      //create friend
      var newFriend = new Friend
      {
        RequesterId = user.Id,
        ReceiverId = ReceiverId,
        FriendStatus = FriendEnum.Pending
      };
      //add database
      await _friendRepository.CreateFriend(newFriend);
      return new ApiResponse<FriendResponse>(201, $"Bạn đã gửi lời mời kết bạn cho người dùng {ReceiverId} thành công!", new FriendResponse
      {
        UserId = ReceiverId,
        FullName = fullName,
        Avatar = user.ImageUrl,
        Address = user.Address,
        Job = user.Job,
        Gender = user.Gender,
        FriendStatus = newFriend.FriendStatus
      });
    }
    public async Task<ApiResponse<List<FriendResponse>>> GetFriendsRequest()
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<List<FriendResponse>>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<List<FriendResponse>>(404, "Người dùng không tồn tại!", null);
      }
      //get friend request
      var friend = await _friendRepository.GetFriendsByUserId(user.Id);
      if (friend == null || friend.Count == 0)
      {
        return new ApiResponse<List<FriendResponse>>(404, "Bạn chưa gửi lời mời kết bạn nào!", null);
      }
      var friendResponse = friend.Select(f =>
      {
        var receiver = f.Receiver;

        return new FriendResponse
        {
          UserId = f.ReceiverId,
          FullName = receiver != null ? $"{receiver.FirstName} {receiver.LastName}" : "Anonymous",
          Avatar = receiver?.ImageUrl ?? "",
          Address = receiver?.Address ?? "",
          Job = receiver?.Job ?? "",
          Gender = receiver?.Gender ?? GenderEnum.Male,
          FriendStatus = f.FriendStatus
        };
      }).ToList();
      return new ApiResponse<List<FriendResponse>>(200, "Lấy thành công danh sách lời mời kết bạn đã gửi!", friendResponse);
    }
  }
}
