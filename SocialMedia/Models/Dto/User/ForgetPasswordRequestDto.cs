using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.User
{
  public class ForgetPasswordRequestDto
  {
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public required string Email { get; set; }
  }
}
