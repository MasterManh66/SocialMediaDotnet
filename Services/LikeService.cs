using SocialMedia.Models.Entities;
using System.Security.Claims;
using SocialMedia.Repositories;
using SocialMedia.Models.Dto.Like;
using SocialMedia.Models.Dto;
using AutoMapper;

namespace SocialMedia.Services
{
  public class LikeService : ILikeService
  {
    private readonly ILikeRepository _likeRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LikeService(IHttpContextAccessor httpContextAccessor, ILikeRepository likeRepository
                  , IPostRepository postRepository, IUserRepository userRepository, IMapper mapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _likeRepository = likeRepository;
      _postRepository = postRepository;
      _userRepository = userRepository;
      _mapper = mapper;
    }
    public string? GetCurrentUserEmail()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
    public Task<User?> GetUserByEmailAsync(string email)
    {
      return _userRepository.GetByEmailAsync(email);
    }
    public Task<Post?> GetPostById(int postId)
    {
      return _postRepository.GetPostById(postId);
    }
    public async Task<ApiResponse<LikeDto>> LikePost(AddLikeRequestDto request)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<LikeDto>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<LikeDto>(404, "Người dùng không tồn tại!", null);
      }
      //check request
      if (request.PostId <= 0)
      {
        return new ApiResponse<LikeDto>(400, "ID bài viết không hợp lệ!", null);
      }
      if (request.PostId == 0)
      {
        return new ApiResponse<LikeDto>(400, "ID bài viết không được để trống!", null);
      }
      var post = await GetPostById(request.PostId);
      if (post == null || request.PostId != post.Id)
      {
        return new ApiResponse<LikeDto>(400, $"Bài viết {request.PostId} không tồn tại!", null);
      }
      var author = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Anonymous";
      //create like
      var newLike = _mapper.Map<Like>(request);
      //add database
      await _likeRepository.AddAsync(newLike);
      var response = _mapper.Map<LikeDto>(newLike);
      return new ApiResponse<LikeDto>(201, $"Bạn đã thích bài viết {request.PostId} của tác giả {author} thành công!", response);
    }
    public async Task<ApiResponse<List<LikeDto>>> LikeOfUser()
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<List<LikeDto>> (401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<List<LikeDto>> (404, "Người dùng không tồn tại!", null);
      }
      //get like of user
      var likes = await _likeRepository.GetLikesByUserId(user.Id);
      if (likes == null || likes.Count == 0)
      {
        return new ApiResponse<List<LikeDto>>(404, "Bạn chưa thích bài viết nào!", null);
      }
      var likeResponses = _mapper.Map<List<LikeDto>>(likes);
      return new ApiResponse<List<LikeDto>>(200, "Danh sách bài viết đã thích của bạn", likeResponses);
    }
    public async Task<ApiResponse<string>> UnlikePost(AddLikeRequestDto request)
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
      //check post
      var post = await GetPostById(request.PostId);
      if (post == null)
      {
        return new ApiResponse<string>(400, $"Bài viết {request.PostId} không tồn tại!", null);
      }
      var author = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Anonymous";
      //check authorize
      var like = await _likeRepository.GetLikeByUserIdAndPostId(user.Id, request.PostId);
      if (like == null)
      {
        return new ApiResponse<string>(404, "Bạn chưa thích bài viết này!", null);
      }
      //delete like
      await _likeRepository.DeleteAsync(request.PostId);
      return new ApiResponse<string>(200, $"Bạn đã huỷ like bài viết {post.Id} của {author} thành công!", null);
    }
  }
}
