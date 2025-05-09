using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialMedia.Models.Domain.Enums;

namespace SocialMedia.Models.Entities
{
  public class User
  {
    [Key]
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? Job { get; set; }
    public string? ImageUrl { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public GenderEnum? Gender { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();

    [InverseProperty("Requester")]
    public ICollection<Friend> FriendRequested { get; set; } = new List<Friend>();
    [InverseProperty("Receiver")]
    public ICollection<Friend> FriendReceived { get; set; } = new List<Friend>();
  }
}
