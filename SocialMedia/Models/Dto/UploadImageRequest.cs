using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto
{
  public class UploadImageRequest
  {
    [Required]
    public required IFormFile Image { get; set; }
  }
}
