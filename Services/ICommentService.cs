using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface ICommentService
  {
    Task<ApiResponse<CommentResponse>> CommentPost(CommentRequest request);
    Task<ApiResponse<List<CommentResponse>>> CommentsOfUser();
    Task<ApiResponse<CommentResponse>> EditComment(CommentEditRequest request);
    Task<ApiResponse<CommentResponse>> DeleteComment(int commentId);
  }
}