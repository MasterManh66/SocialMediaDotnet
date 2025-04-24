using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
  public class Comment
  {
    [Key]
    public int Id { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("PostId")]
    [Required]
    public int PostId { get; set; }
    public Post? Post { get; set; }

    [ForeignKey("UserId")]
    [Required]
    public int UserId { get; set; }
    public User? User { get; set; }
  }
}
