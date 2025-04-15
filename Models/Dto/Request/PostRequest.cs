namespace SocialMedia.Models.Dto.Request
{
  public class PostRequest
  {
    public string? Title { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
  }
}
