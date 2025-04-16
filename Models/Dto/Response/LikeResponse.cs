namespace SocialMedia.Models.Dto.Response
{
  public class LikeResponse
  {
    public int Id { get; set; }
    public int PostId { get; set; }
    public string? PostTitle { get; set; }
    public int UserId { get; set; }
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}
