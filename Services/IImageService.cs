using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface IImageService
  {
    Task<ApiResponse<string>> UploadImage(UploadImageRequest request);
    Task<ApiResponse<string>> DownloadImage(string imageUrl);
    Task<ApiResponse<string>> DeleteImage(string imageUrl);
  }
}
