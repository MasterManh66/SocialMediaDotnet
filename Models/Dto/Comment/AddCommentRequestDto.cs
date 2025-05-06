using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Dto.Comment
{
  public class AddCommentRequestDto
  {
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }

    [Required(ErrorMessage = "PostId không được để trống!")]
    [Range(1, int.MaxValue, ErrorMessage = "PostId bài viết không hợp lệ!")]
    public int PostId { get; set; }
  }
}
