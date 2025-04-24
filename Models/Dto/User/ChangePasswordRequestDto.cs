using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.User
{
  public class ChangePasswordRequestDto
  {
    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public required string NewPassword { get; set; }

    [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu xác nhận phải có ít nhất 6 ký tự")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public required string ConfirmPassword { get; set; }
  }
}
