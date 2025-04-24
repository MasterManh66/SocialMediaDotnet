using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.Like;

namespace SocialMedia.Services
{
  public interface ILikeService
  {
    Task<ApiResponse<LikeDto>> LikePost(AddLikeRequestDto request);
    Task<ApiResponse<List<LikeDto>>> LikeOfUser();
    Task<ApiResponse<string>> UnlikePost(AddLikeRequestDto request);
  }
}
