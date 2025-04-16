using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Dto.Response
{
  public class FriendResponse
  {
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string? Avatar { get; set; }
    public string? Address { get; set; }
    public string? Job { get; set; }
    public GenderEnum? Gender { get; set; }
    public FriendEnum? FriendStatus { get; set; }
  }
}
