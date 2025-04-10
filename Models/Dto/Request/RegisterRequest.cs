using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Request
{
  public class RegisterRequest
  {
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password không được để trống")]
    [MinLength(6, ErrorMessage = "Password phải có ít nhất 6 ký tự")]
    public required string Password { get; set; }
  }
}
