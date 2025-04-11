using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Response
{
  public class AuthResponse
  {
    [Required]
    public string Token { get; set; } = string.Empty;
  }
}
