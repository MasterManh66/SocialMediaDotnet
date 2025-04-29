using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Comment
{
  public class AddCommentRequestDto
  {
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }

    [MinLength(1, ErrorMessage = "PostId bài viết không hợp lệ!")]
    public int PostId { get; set; }
  }
}
