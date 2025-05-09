using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Comment
{
  public class UpdateCommentRequestDto
  {
    [Required(ErrorMessage = "CommentId không được để trống.")]
    public int CommentId { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
  }
}
