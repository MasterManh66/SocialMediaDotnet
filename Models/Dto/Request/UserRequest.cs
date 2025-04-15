using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Dto.Request
{
  public class UserRequest
  {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Job { get; set; } 
    public IFormFile? ImageUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public GenderEnum? Gender { get; set; }
  }
}
