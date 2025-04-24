namespace SocialMedia.Models.Dto.Comment
{
  public class UpdateCommentRequestDto
  {
    public int Id { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
  }
}
