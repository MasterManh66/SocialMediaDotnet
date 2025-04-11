using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Request
{
  public class ForgetPasswordRequest
  {
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public required string Email { get; set; }
  }
}
