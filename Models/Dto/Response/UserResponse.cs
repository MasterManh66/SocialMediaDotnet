using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Dto.Response
{
  public class UserResponse
  {
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; } 
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; } 
    public string? Job { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public GenderEnum? Gender { get; set; }
  }
}
