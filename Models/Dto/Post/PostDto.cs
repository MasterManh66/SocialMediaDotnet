using SocialMedia.Models.Domain.Enums;

namespace SocialMedia.Models.Dto.Post
{
  public class PostDto
  {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public PostEnum? PostStatus { get; set; }
    public int UserId { get; set; }
    public string? Author { get; set; }
  }
}
