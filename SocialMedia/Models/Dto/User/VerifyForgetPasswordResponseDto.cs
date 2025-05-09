namespace SocialMedia.Models.Dto.User
{
  public class VerifyForgetPasswordResponseDto
  {
    public string Link { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
  }
}
