using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Request
{
  public class LikeRequest
  {
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "PostId là một số nguyên dương!")]
    public int PostId { get; set; }
  }
}
