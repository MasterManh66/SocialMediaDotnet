using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface IPostService
  {
    Task<ApiResponse<PostResponse>> CreatePost(PostCreateRequest request);
    Task<ApiResponse<PostResponse>> UpdatePost(PostEditRequest request);
    Task<ApiResponse<List<PostResponse>>> GetPost();
    Task<ApiResponse<string>> DeletePost(int postId);
    Task<ApiResponse<List<PostResponse>>> SearchPost(string keyWord);
  }
}
