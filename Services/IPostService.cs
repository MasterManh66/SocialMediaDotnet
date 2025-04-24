using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.Post;

namespace SocialMedia.Services
{
  public interface IPostService
  {
    Task<ApiResponse<PostDto>> CreatePost(AddPostRequestDto request);
    Task<ApiResponse<PostDto>> UpdatePost(UpdatePostRequestDto request);
    Task<ApiResponse<List<PostDto>>> GetPost();
    Task<ApiResponse<string>> DeletePost(int postId);
    Task<ApiResponse<List<PostDto>>> SearchPost(string keyWord);
    Task<ApiResponse<List<PostDto>>> GetTimeLine();
  }
}
