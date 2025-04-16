using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Entities
{
  public class UserRole
  {
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
