using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialMedia.Models.Domain.Enums;

namespace SocialMedia.Models.Entities
{
  public class Post
  {
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public PostEnum? PostStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
    public ICollection<Like>? Likes { get; set; } = new List<Like>();
  }
}
