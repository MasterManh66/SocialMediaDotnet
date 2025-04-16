namespace SocialMedia.Models.Dto.Request
{
  public class CommentEditRequest
  {
    public int Id { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
  }
}
