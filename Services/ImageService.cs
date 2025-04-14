using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public class ImageService : IImageService
  {
    private readonly string[] _allowedExtensions = { ".jpg", ".png", ".jpeg", ".gif" };
    private readonly string _imagePath;
    private readonly long _maxFileSize = 5 * 1024 * 1024;

    public ImageService(IWebHostEnvironment env)
    {
      _imagePath = Path.Combine(env.ContentRootPath, "Uploads");
      if (!Directory.Exists(_imagePath))
      {
        Directory.CreateDirectory(_imagePath);
      }
    }
    public async Task<ApiResponse<string>> UploadImage(UploadImageRequest request)
    {
      var file = request.Image;
      if (file == null || file.Length == 0)
      {
        return new ApiResponse<string>(400, "Không có tệp nào được tải lên!", null);
      }
      if (file.Length > _maxFileSize)
      {
        return new ApiResponse<string>(400, "Kích thước tệp quá lớn! Kích thước tối đa là 5MB.", null);
      }
      var fileExtension = Path.GetExtension(file.FileName).ToLower();
      if (!_allowedExtensions.Contains(fileExtension))
      {
        return new ApiResponse<string>(400, "Định dạng tệp không hợp lệ! Chỉ cho phép jpg, png, jpeg, gif.", null);
      }
      var fileName = Guid.NewGuid() + fileExtension;
      var filePath = Path.Combine(_imagePath, fileName);
      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }
      return new ApiResponse<string>(201, "Tải ảnh lên thành công!", $"/Uploads/{fileName} ");
    }
  }
}
