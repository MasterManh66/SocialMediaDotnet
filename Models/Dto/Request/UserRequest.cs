using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Dto.Request
{
  public class UserRequest
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public GenderEnum Gender { get; set; }
  }
}
