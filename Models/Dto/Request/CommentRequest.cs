namespace SocialMedia.Models.Dto.Request
{
  public class CommentRequest
  {
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public int PostId { get; set; }
  }
}
