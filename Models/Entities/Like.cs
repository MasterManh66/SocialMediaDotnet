using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models.Entities
{
  public class Like
  {
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [ForeignKey("PostId")]
    public int PostId { get; set; }
    public Post? Post { get; set; }

    [Required]
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public User? User { get; set; }
  }
}
