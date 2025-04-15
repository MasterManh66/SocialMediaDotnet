using System.Security.Claims;
using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;

namespace SocialMedia.Services
{
  public class PostService : IPostService
  {
    private readonly IUserService _userService;
    private readonly IPostRepository _postRepository;
    private readonly IImageService _imageService;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostService(IUserRepository userRepository,IUserService userService, IPostRepository postRepository, 
                      IImageService imageService, IHttpContextAccessor httpContextAccessor)
    {
      _userService = userService;
      _postRepository = postRepository;
      _userRepository = userRepository;
      _imageService = imageService;
      _httpContextAccessor = httpContextAccessor;
    }
    public string? GetCurrentUserEmail()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
    public Task<User?> GetUserByEmailAsync(string email)
    {
      return _userRepository.GetByEmailAsync(email);
    }
    public async Task<ApiResponse<PostResponse>> CreatePostAsync(PostRequest request)
    {
      //check user
      var enail = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(enail))
      {
        return new ApiResponse<PostResponse>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(enail);
      if (user == null)
      {
        return new ApiResponse<PostResponse>(404, "Người dùng không tồn tại!", null);
      }
      //check request
      if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
      {
        return new ApiResponse<PostResponse>(400, "Tiêu đề hoặc Nội dung đang bị trống!", null);
      }
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<PostResponse>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      return new ApiResponse<PostResponse>(201, "Bạn đã tạo bài viết thành công!", new PostResponse
        {

        });
    }
  }
}
