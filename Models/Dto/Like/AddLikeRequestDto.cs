using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Like
{
  public class AddLikeRequestDto
  {
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "PostId là một số nguyên dương!")]
    public int PostId { get; set; }
  }
}
