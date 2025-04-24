namespace SocialMedia.Models.Dto.Comment
{
  public class AddCommentRequestDto
  {
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public int PostId { get; set; }
  }
}
