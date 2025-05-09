using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.User
{
  public class AuthUserResponseDto
  {
    [Required]
    public string Token { get; set; } = string.Empty;
  }
}
