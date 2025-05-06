using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.Comment;

namespace SocialMedia.Services
{
  public interface ICommentService
  {
    Task<ApiResponse<CommentDto>> CreateComment(AddCommentRequestDto request);
    Task<ApiResponse<List<CommentDto>>> CommentsOfUser();
    Task<ApiResponse<CommentDto>> EditComment(UpdateCommentRequestDto request);
    Task<ApiResponse<CommentDto>> DeleteComment(int commentId);
  }
}