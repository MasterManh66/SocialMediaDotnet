using SocialMedia.Models.Domain.Enums;

namespace SocialMedia.Models.Dto.User
{
  public class UpdateUserRequestDto
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
