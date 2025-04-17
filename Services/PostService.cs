using System.Security.Claims;
using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Entities;
using SocialMedia.Models.Enums;
using SocialMedia.Repositories;

namespace SocialMedia.Services
{
  public class PostService : IPostService
  {
    private readonly IUserService _userService;
    private readonly IPostRepository _postRepository;
    private readonly IImageService _imageService;
    private readonly IUserRepository _userRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostService(IUserRepository userRepository,IUserService userService, IPostRepository postRepository, 
                      IImageService imageService, IHttpContextAccessor httpContextAccessor, IFriendRepository friendRepository)
    {
      _userService = userService;
      _postRepository = postRepository;
      _userRepository = userRepository;
      _imageService = imageService;
      _friendRepository = friendRepository;
      _httpContextAccessor = httpContextAccessor;
    }
    public string? GetCurrentUserEmail()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
    public Task<User?> GetUserByEmailAsync(string email)
    {
      return _userRepository.GetByEmailAsync(email);
    }
    public async Task<ApiResponse<PostResponse>> CreatePost(PostCreateRequest request)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<PostResponse>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<PostResponse>(404, "Người dùng không tồn tại!", null);
      }
      var infoUser = await _userRepository.GetUserById(user.Id);
      string author = $"{infoUser.FirstName} {infoUser.LastName}";
      //check request
      if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
      {
        return new ApiResponse<PostResponse>(400, "Tiêu đề hoặc Nội dung đang bị trống!", null);
      }
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<PostResponse>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      //created Post
      var newPost = new Post
      {
        Title = request.Title,
        Content = request.Content,
        ImageUrl = imageUrl,
        UserId = user.Id,
        PostStatus = request.PostStatus
      };
      //add database
      await _postRepository.CreatePost(newPost);
      return new ApiResponse<PostResponse>(201, "Bạn đã tạo bài viết thành công!", new PostResponse
        {
          Id = newPost.Id,
          Title = newPost.Title,
          Content = newPost.Content,
          ImageUrl = newPost.ImageUrl,
          PostStatus = newPost.PostStatus,
          UserId = newPost.UserId,
          Author = author
      });
    }
    public async Task<ApiResponse<PostResponse>> UpdatePost(PostEditRequest request)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<PostResponse>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<PostResponse>(404, "Người dùng không tồn tại!", null);
      }
      var infoUser = await _userRepository.GetUserById(user.Id);
      string author = $"{infoUser.FirstName} {infoUser.LastName}";
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<PostResponse>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      //update post
      bool isUpdate = false;
      var post = await _postRepository.GetPostById(request.Id);
      if (post == null)
      {
        return new ApiResponse<PostResponse>(400, $"Bài viết {request.Id} không tồn tại!", null);
      }
      if (user.Id != post.UserId)
      {
        return new ApiResponse<PostResponse>(403, $"Bạn không có quyền chỉnh sửa bài viết của {user.FirstName + "" + user.LastName}", null);
      }
      if (!string.IsNullOrEmpty(request.Title) && request.Title != post.Title)
      {
        post.Title = request.Title;
        isUpdate = true;
      }
      if (!string.IsNullOrEmpty(request.Content) && request.Content != post.Content)
      {
        post.Content = request.Content;
        isUpdate = true;
      }
      if (!string.IsNullOrEmpty(imageUrl) && imageUrl != post.ImageUrl)
      {
        post.ImageUrl = imageUrl;
        isUpdate = true;
      }
      if (request.PostStatus != post.PostStatus)
      {
        post.PostStatus = request.PostStatus;
        isUpdate = true;
      }
      if (isUpdate)
      {
        await _postRepository.UpdatePost(post);
      }
      return new ApiResponse<PostResponse>(200, $"Chỉnh sửa bài viết {post.Id} thành công !", new PostResponse
      {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        ImageUrl = post.ImageUrl,
        PostStatus = post.PostStatus,
        UserId = post.UserId,
        Author = author
      });
    }
    public async Task<ApiResponse<List<PostResponse>>> GetPost()
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<List<PostResponse>>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      
      if (user == null)
      {
        return new ApiResponse<List<PostResponse>>(404, "Người dùng không tồn tại!", null);
      }
      //Get List Post By User
      var post = await _postRepository.GetPostsByUserId(user.Id);
      var infoUser = await _userRepository.GetUserById(user.Id);
      string author = $"{infoUser.FirstName} {infoUser.LastName}";
      var postResponses = post.Select(p => new PostResponse
      {
        Id = p.Id,
        Title = p.Title,
        Content = p.Content,
        ImageUrl = p.ImageUrl,
        PostStatus = p.PostStatus,
        UserId = p.UserId,
        Author = author
      }).ToList();
      return new ApiResponse<List<PostResponse>>(200, $"Lấy thành công danh sách bài viết của người dùng {author}!", postResponses);
    }
    public async Task<ApiResponse<string>> DeletePost(int postId)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<string>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<string>(404, "Người dùng không tồn tại!", null);
      }
      //check authorize
      var post = await _postRepository.GetPostById(postId);
      if (post == null)
      {
        return new ApiResponse<string>(400, $"Bài viết {postId} không tồn tại!", null);
      }
      if (user.Id != post.UserId)
      {
        return new ApiResponse<string>(403, $"Bạn không có quyền xoá bài viết {postId} này!", null);
      }
      //delete post
      await _postRepository.DeletePost(postId);
      return new ApiResponse<string>(200, $"Bạn đã xoá bài viết {postId} thành công!", null);
    }
    public async Task<ApiResponse<List<PostResponse>>> SearchPost(string keyWord)
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<List<PostResponse>>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<List<PostResponse>>(404, "Người dùng không tồn tại!", null);
      }
      //check request
      if (string.IsNullOrWhiteSpace(keyWord))
      {
        return new ApiResponse<List<PostResponse>>(400, "Từ khoá tìm kiếm đang bị trống hoặc là khoảng trắng!", null);
      }
      //search
      var post = await _postRepository.SearchPostByKey(keyWord);
      var postResponses = post.Select(p => new PostResponse
      {
        Id = p.Id,
        Title = p.Title,
        Content = p.Content,
        ImageUrl = p.ImageUrl,
        PostStatus = p.PostStatus,
        UserId = p.UserId,
        Author = (p.User != null) ? $"{p.User.FirstName} {p.User.LastName}" : "Anonymous"
      }).ToList();
      if (postResponses.Count > 0) 
      {
        return new ApiResponse<List<PostResponse>>(200, $"Tìm kiếm theo từ khoá {keyWord} thành công !", postResponses);
      } else
      {
        return new ApiResponse<List<PostResponse>>(204, $"Từ khoá {keyWord} không có kết quả trùng hợp !", postResponses);
      }
    }
    public async Task<ApiResponse<List<PostResponse>>> GetTimeLine()
    {
      //check user
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<List<PostResponse>>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<List<PostResponse>>(404, "Người dùng không tồn tại!", null);
      }
      //check post of user
      var postUser = await _postRepository.GetPostsByUserId(user.Id);
      var postUserFiltered = postUser.ToList();
      //check friend of user
      var friendUser = await _friendRepository.GetFriendsByUserId(user.Id);
      var listFriend = friendUser.Where(f => f.FriendStatus == FriendEnum.Accepted).ToList();
      //get post of friend
      var friend = listFriend.Select(f => f.RequesterId == user.Id ? f.ReceiverId : f.RequesterId).ToList();
      var postFriend = await _postRepository.GetPostsByUserIds(friend);
      var postFriendFiltered = postFriend.Where(p => p.PostStatus == PostEnum.Public || p.PostStatus == PostEnum.Friends).ToList();
      //Concat post of user and friend
      var allPosts = postUserFiltered.Concat(postFriendFiltered).OrderByDescending(p => p.CreatedAt).ToList();
      if (!allPosts.Any())
      {
        return new ApiResponse<List<PostResponse>>(404, "Timeline không có bài viết mới nào!", null);
      }
      var postResponse = allPosts.Select(p => new PostResponse
      {
        Id = p.Id,
        Title = p.Title,
        Content = p.Content,
        ImageUrl = p.ImageUrl,
        UserId = p.UserId,
        Author = "Anonymus",
        PostStatus = p.PostStatus
      }).ToList();
      return new ApiResponse<List<PostResponse>>(200, "Lấy thành công danh sách Time Line của bạn!", postResponse);
    }
  }
}
