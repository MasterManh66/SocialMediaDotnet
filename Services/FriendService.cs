using SocialMedia.Models.Entities;
using System.Security.Claims;
using SocialMedia.Repositories;
using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Enums;
using Azure.Core;

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
      if (friend != null && friend.Any(f => (f.RequesterId == user.Id && f.ReceiverId == ReceiverId) || (f.RequesterId == ReceiverId && f.ReceiverId == user.Id)))
      {
        return new ApiResponse<FriendResponse>(400, $"Bạn đã gửi lời mời kết bạn cho người dùng {ReceiverId} trước đó!", null);
      }
      if (friend != null && friend.Any(f => f.FriendStatus == FriendEnum.Accepted && ((f.RequesterId == user.Id && f.ReceiverId == ReceiverId) 
                                                                                  || (f.RequesterId == ReceiverId && f.ReceiverId == user.Id))))
      {
        return new ApiResponse<FriendResponse>(400, $"Bạn đã là bạn bè với người dùng {ReceiverId}!", null);
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
        FullName = $"{receiver.FirstName} {receiver.LastName}",
        Avatar = receiver.ImageUrl,
        Address = receiver.Address,
        Job = receiver.Job,
        Gender = receiver.Gender,
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
      if (friend == null || friend.Count == 0 || friend.All(f => f.FriendStatus != FriendEnum.Pending))
      {
        return new ApiResponse<List<FriendResponse>>(404, "Bạn chưa gửi lời mời kết bạn nào!", null);
      }
      var sentRequests = friend.Where(f => f.FriendStatus == FriendEnum.Pending && f.RequesterId == user.Id).ToList();
      if (sentRequests.Count == 0)
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
    public async Task<ApiResponse<List<FriendResponse>>> GetFriendsReceiver()
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
      //check friend receiver
      var friend = await _friendRepository.GetFriendsByUserId(user.Id);
      if (friend == null || friend.Count == 0 || friend.All(f => f.FriendStatus != FriendEnum.Pending))
      {
        return new ApiResponse<List<FriendResponse>>(404, "Bạn chưa nhận lời mời kết bạn nào!", null);
      }
      var requestReceiver = friend.Where(f => f.FriendStatus == FriendEnum.Pending && f.ReceiverId == user.Id).ToList();
      if (requestReceiver.Count == 0)
      {
        return new ApiResponse<List<FriendResponse>>(404, "Bạn chưa nhận được yêu cầu kết bạn nào!", null);
      }
      //get friend receiver
      var friendResponse = friend.Select(f =>
      {
        var requester = f.Requester;
        return new FriendResponse
        {
          UserId = f.RequesterId,
          FullName = requester != null ? $"{requester.FirstName} {requester.LastName}" : "Anonymous",
          Avatar = requester?.ImageUrl ?? "",
          Address = requester?.Address ?? "",
          Job = requester?.Job ?? "",
          Gender = requester?.Gender ?? GenderEnum.Male,
          FriendStatus = f.FriendStatus
        };
      }).ToList();
      return new ApiResponse<List<FriendResponse>>(200, "Lấy thành công danh sách lời mời kết bạn đã nhận!", friendResponse);
    }
    public async Task<ApiResponse<FriendResponse>> AcceptedFriend(int RequestId)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<FriendResponse>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<FriendResponse>(404, "Người dùng không tồn tại!", null);
      }
      //check request
      if (RequestId <= 0)
      {
        return new ApiResponse<FriendResponse>(400, "Người dùng không hợp lệ!", null);
      }
      if (RequestId == user.Id)
      {
        return new ApiResponse<FriendResponse>(400, "Bạn không thể gửi đến chính mình!", null);
      }
      //check user request
      var request = await _userRepository.GetUserById(RequestId);
      if (request == null)
      {
        return new ApiResponse<FriendResponse>(404, $"Người dùng {RequestId} không tồn tại!", null);
      }
      //check friend
      var friend = await _friendRepository.GetFriendsByUserId(user.Id);
      if (friend != null && friend.Any(f => f.FriendStatus == FriendEnum.Accepted &&
          ((f.RequesterId == user.Id && f.ReceiverId == RequestId) || (f.ReceiverId == user.Id && f.RequesterId == RequestId))))
      {
        return new ApiResponse<FriendResponse>(400, $"Bạn với người dùng {RequestId} đã là bạn bè!", null);
      }
      var pendingRequest = friend?.FirstOrDefault(f => f.FriendStatus == FriendEnum.Pending && f.RequesterId == RequestId && f.ReceiverId == user.Id);
      if (pendingRequest == null)
      {
        return new ApiResponse<FriendResponse>(404, $"Không tìm thấy lời mời kết bạn từ người dùng {RequestId}!", null);
      }
      pendingRequest.FriendStatus = FriendEnum.Accepted;
      await _friendRepository.UpdateFriend(pendingRequest);

      var sender = pendingRequest.Requester;
      if (sender != null)
      {
        var friendResponse = new FriendResponse
        {
          UserId = sender.Id,
          FullName = $"{sender.FirstName} {sender.LastName}",
          Avatar = sender.ImageUrl,
          Address = sender.Address,
          Job = sender.Job,
          Gender = sender.Gender,
          FriendStatus = pendingRequest.FriendStatus
        };
        return new ApiResponse<FriendResponse>(200, $"Đã chấp nhận lời mời kết bạn của {sender.FirstName} {sender.LastName}!", friendResponse);
      }
      return new ApiResponse<FriendResponse>(400, "Kết bạn không thành công", null);
    }
    public async Task<ApiResponse<List<FriendResponse>>> FriendOfUser()
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
      //check friend
      var friend = await _friendRepository.GetFriendsByUserId(user.Id);
      if (friend == null || friend.Count == 0 || friend.All(f => f.FriendStatus != FriendEnum.Accepted))
      {
        return new ApiResponse<List<FriendResponse>>(404, "Bạn chưa kết bạn với ai!", null);
      }
      var listFriend = friend.Where(f => f.FriendStatus == FriendEnum.Accepted && ((f.RequesterId == user.Id) || (f.ReceiverId == user.Id))).ToList();
      if (listFriend.Count == 0)
      {
        return new ApiResponse<List<FriendResponse>>(404, "Danh sách bạn bè trống.", null);
      }
      //get list friend of user
      var friendResponse = listFriend.Select(f =>
      {
        var friendUser = f.RequesterId == user.Id ? f.Receiver! : f.Requester!;
          return new FriendResponse
          {
            UserId = friendUser.Id,
            FullName = $"{friendUser.FirstName} {friendUser.LastName}",
            Avatar = friendUser.ImageUrl,
            Address = friendUser.Address,
            Job = friendUser.Job,
            Gender = friendUser.Gender,
            FriendStatus = f.FriendStatus
          };
      }).ToList();
      return new ApiResponse<List<FriendResponse>>(200, "Lấy thành công danh sách bạn bè của bạn!", friendResponse);
    }
    public async Task<ApiResponse<string>> DeleteFriend(int FriendId)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<string>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<string>(404, "Người dùng không tồn tại!", null);
      }
      //check request
      if (FriendId <= 0 && FriendId == user.Id)
      {
        return new ApiResponse<string>(400, "Người dùng không hợp lệ!", null);
      }
      //check user request
      var request = await _userRepository.GetUserById(FriendId);
      if (request == null)
      {
        return new ApiResponse<string>(404, $"Người dùng {FriendId} không tồn tại!", null);
      }
      //check friend
      var friend = await _friendRepository.GetFriendsByUserId(user.Id);
      if (friend != null && friend.Any(f => f.FriendStatus == FriendEnum.Pending &&
          ((f.RequesterId == user.Id && f.ReceiverId == FriendId) || (f.ReceiverId == user.Id && f.RequesterId == FriendId))))
      {
        return new ApiResponse<string>(400, $"Bạn với người dùng {FriendId} không phải là bạn bè!", null);
      }
      var pendingRequest = friend?.FirstOrDefault(f => f.FriendStatus == FriendEnum.Accepted && ((f.RequesterId == user.Id && f.ReceiverId == FriendId) || (f.ReceiverId == user.Id && f.RequesterId == FriendId)));
      if (pendingRequest == null)
      {
        return new ApiResponse<string>(404, $"Không tìm thấy người bạn {FriendId} này!", null);
      }
      var infoFriend = await _userRepository.GetUserById(pendingRequest.Id);
      if (infoFriend == null)
      {
        return new ApiResponse<string>(404, $"Không có thông tin người dùng {FriendId} này!", null);
      }
      await _friendRepository.DeleteFriend(pendingRequest.Id);
      return new ApiResponse<string>(200, $"Huỷ kết bạn thành công với người dùng {infoFriend.FirstName} {infoFriend.LastName}", null);
    }
  }
}
