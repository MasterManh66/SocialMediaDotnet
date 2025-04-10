using System.ComponentModel.DataAnnotations;
using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Entities
{
  public class User
  {
    [Key]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Job { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public GenderEnum Gender { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
  }
}
