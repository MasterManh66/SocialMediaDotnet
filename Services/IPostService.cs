using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface IPostService
  {
    Task<ApiResponse<PostResponse>> CreatePostAsync(PostRequest request);
  }
}
