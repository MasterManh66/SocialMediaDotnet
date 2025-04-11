using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Request
{
  public class VerifyForgetPasswordRequest
  {
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "OTP không được để trống")]
    [MaxLength(6, ErrorMessage = "OTP chỉ có 6 chữ số")]
    public required string Otp { get; set; }
  }
}
