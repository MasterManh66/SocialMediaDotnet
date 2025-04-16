using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface ILikeService
  {
    Task<ApiResponse<LikeResponse>> LikePost(LikeRequest request);
    Task<ApiResponse<List<LikeResponse>>> LikeOfUser();
    Task<ApiResponse<string>> UnlikePost(LikeRequest request);
  }
}
