using System.ComponentModel.DataAnnotations;
using SocialMedia.Models.Enums;

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
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int UserId { get; set; }
    public User? User { get; set; }
  }
}
