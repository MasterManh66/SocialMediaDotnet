using SocialMedia.Models.Dto;

namespace SocialMedia.Services
{
  public interface IImageService
  {
    Task<ApiResponse<string>> UploadImage(UploadImageRequest request);
    Task<ApiResponse<string>> DownloadImage(string imageUrl);
    Task<ApiResponse<string>> DeleteImage(string imageUrl);
  }
}
