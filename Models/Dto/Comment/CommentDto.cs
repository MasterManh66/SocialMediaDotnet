namespace SocialMedia.Models.Dto.Comment
{
  public class CommentDto
  {
    public int Id { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public string? Author { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
  }
}
