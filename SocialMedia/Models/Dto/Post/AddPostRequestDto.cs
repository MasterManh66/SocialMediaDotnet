using System.ComponentModel.DataAnnotations;
using SocialMedia.Models.Domain.Enums;

namespace SocialMedia.Models.Dto.Post
{
  public class AddPostRequestDto
  {
    public string? Title { get; set; }
    public string? Content { get; set; }
    public IFormFile? ImageUrl { get; set; }
    [Required(ErrorMessage = "Trạng thái bài viết không được để trống")]
    public PostEnum? PostStatus { get; set; }
  }
}
