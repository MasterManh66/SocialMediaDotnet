using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Entities
{
  public class Friend
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Requester")]
    public int RequesterId { get; set; }

    [InverseProperty("FriendRequested")]
    public User? Requester { get; set; }

    [ForeignKey("Receiver")]
    public int ReceiverId { get; set; }

    [InverseProperty("FriendReceived")]
    public User? Receiver { get; set; }

    public FriendEnum FriendStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
