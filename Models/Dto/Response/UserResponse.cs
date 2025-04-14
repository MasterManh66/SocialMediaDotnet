using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Dto.Response
{
  public class UserResponse
  {
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }

    public GenderEnum Gender { get; set; }
  }
}
