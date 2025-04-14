using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Request
{
  public class UploadImageRequest
  {
    [Required]
    public required IFormFile Image { get; set; }
  }
}
