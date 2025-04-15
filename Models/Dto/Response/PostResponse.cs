namespace SocialMedia.Models.Dto.Response
{
  public class PostResponse
  {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
  }
}
