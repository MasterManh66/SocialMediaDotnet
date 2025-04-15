using SocialMedia.Models.Enums;

namespace SocialMedia.Models.Dto.Request
{
  public class PostEditRequest
  {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public PostEnum? PostStatus { get; set; }
  }
}
