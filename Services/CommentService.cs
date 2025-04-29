using SocialMedia.Models.Entities;
using System.Security.Claims;
using SocialMedia.Repositories;
using SocialMedia.Models.Dto.Comment;
using SocialMedia.Models.Dto;
using AutoMapper;

namespace SocialMedia.Services
{
  public class CommentService : ICommentService
  {
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CommentService(IHttpContextAccessor httpContextAccessor, ICommentRepository commentRepository, IUserRepository userRepository
                      , IPostRepository postRepository, IUserService userService, IImageService imageService, IMapper mapper)
    {
      _httpContextAccessor = httpContextAccessor;
      _commentRepository = commentRepository;
      _userRepository = userRepository;
      _postRepository = postRepository;
      _userService = userService;
      _imageService = imageService;
      _mapper = mapper;
    }
    public string? GetCurrentUserEmail()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
    public Task<User?> GetUserByEmailAsync(string email)
    {
      return _userRepository.GetByEmailAsync(email);
    }
    public async Task<ApiResponse<CommentDto>> CommentPost(AddCommentRequestDto request)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<CommentDto>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<CommentDto>(404, "Người dùng không tồn tại!", null);
      }
      //check post
      var post = await _postRepository.GetPostById(request.PostId);
      if (post == null || request.PostId != post.Id)
      {
        return new ApiResponse<CommentDto>(404, "Bài viết không tồn tại!", null);
      }
      var author = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Anonymous";
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<CommentDto>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      //created Comment
      var newComment = _mapper.Map<Comment>(request);
      //add database
      await _commentRepository.AddAsync(newComment);
      var response = _mapper.Map<CommentDto>(newComment);
      return new ApiResponse<CommentDto>(201, $"Bạn đã bình luận bài viết {post.Id} thành công!", response);
    }
    public async Task<ApiResponse<List<CommentDto>>> CommentsOfUser()
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<List<CommentDto>>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<List<CommentDto>>(404, "Người dùng không tồn tại!", null);
      }
      //get comment
      var comments = await _commentRepository.GetCommentsByUserId(user.Id);
      if (comments == null || comments.Count == 0)
      {
        return new ApiResponse<List<CommentDto>>(404, "Người dùng chưa bình luận bài viết nào!", null);
      }
      //get comment
      var commentResponses = _mapper.Map<List<CommentDto>>(comments);
      return new ApiResponse<List<CommentDto>>(200, "Lấy danh sách bình luận thành công!", commentResponses);
    }
    public async Task<ApiResponse<CommentDto>> EditComment(UpdateCommentRequestDto request)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<CommentDto>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<CommentDto>(404, "Người dùng không tồn tại!", null);
      }
      //check comment
      var comment = await _commentRepository.GetCommentById(request.Id);
      if (comment == null || request.Id != comment.Id)
      {
        return new ApiResponse<CommentDto>(404, "Bình luận không tồn tại!", null);
      }
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<CommentDto>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      //edit comment
      bool isUpdate = false;
      if (!string.IsNullOrEmpty(request.Content) && request.Content != comment.Content)
      {
        comment.Content = request.Content;
        isUpdate = true;
      }
      if (imageUrl != null && imageUrl != comment.ImageUrl)
      {
        comment.ImageUrl = imageUrl;
        isUpdate = true;
      }
      //add database
      if (isUpdate)
      {
        await _commentRepository.UpdateAsync(comment);
      }
      else
      {
        return new ApiResponse<CommentDto>(400, $"Comment {comment.Id} không có sự thay đổi gì!", null);
      }
      var response = _mapper.Map<CommentDto>(comment);
      return new ApiResponse<CommentDto>(200, $"Bạn đã sửa bình luận bài viết {comment.PostId} thành công!", response);
    }
    public async Task<ApiResponse<CommentDto>> DeleteComment(int commentId)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<CommentDto>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<CommentDto>(404, "Người dùng không tồn tại!", null);
      }
      //check requet
      if (commentId <= 0 || commentId == 0)
      {
        return new ApiResponse<CommentDto>(400, "Comment ID không hợp lệ!", null);
      }
      //check comment
      var comment = await _commentRepository.GetCommentById(commentId);
      if (comment == null || commentId != comment.Id)
      {
        return new ApiResponse<CommentDto>(404, "Bình luận không tồn tại!", null);
      }
      if (comment.UserId != user.Id)
      {
        return new ApiResponse<CommentDto>(403, "Bạn không có quyền xóa bình luận này!", null);
      }
      //delete comment
      await _commentRepository.DeleteAsync(commentId);
      return new ApiResponse<CommentDto>(204, $"Bạn đã xóa bình luận bài viết {comment.PostId} thành công!", null);
    }
  }
}
