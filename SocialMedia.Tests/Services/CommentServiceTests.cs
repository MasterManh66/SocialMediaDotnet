using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SocialMedia.Models.Dto.Comment;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using SocialMedia.Services;

namespace SocialMedia.Tests.Services
{
  public class CommentServiceTests
  {
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
      _commentRepositoryMock = new Mock<ICommentRepository>();
      _postRepositoryMock = new Mock<IPostRepository>();
      _userRepositoryMock = new Mock<IUserRepository>();
      _imageServiceMock = new Mock<IImageService>();
      _mapperMock = new Mock<IMapper>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

      _commentService = new CommentService(
        _httpContextAccessorMock.Object,
        _commentRepositoryMock.Object,
        _userRepositoryMock.Object,
        _postRepositoryMock.Object,
        _imageServiceMock.Object,
        _mapperMock.Object
      );
    }

    [Fact]
    public async Task CreateComment_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email };
      var request = new AddCommentRequestDto
      {
        PostId = 1,
        Content = "Test comment",
        ImageUrl = null
      };
      var post = new Post
      {
        Id = 1,
        Content = "Test post",
        UserId = user.Id
      };
      var comment = new Comment
      {
        Id = 1,
        Content = request.Content,
        PostId = request.PostId,
        UserId = user.Id
      };
      var response = new CommentDto
      {
        Id = comment.Id,
        Content = comment.Content,
        PostId = comment.PostId,
        UserId = comment.UserId
      };
      // claim
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claims, "TestAuth");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
      _postRepositoryMock.Setup(r => r.GetPostById(request.PostId)).ReturnsAsync(post);
      _mapperMock.Setup(m => m.Map<Comment>(request)).Returns(comment);
      _mapperMock.Setup(m => m.Map<CommentDto>(comment)).Returns(response);
      // Act
      var result = await _commentService.CreateComment(request);
      // Assert
      result.Status.Should().Be(201);
      result.Message.Should().Be($"Bạn đã bình luận bài viết {post.Id} thành công!");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetComment_Authorize_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email };
      var comment = new Comment
      {
        Id = 1,
        Content = "Test comment",
        PostId = 1,
        UserId = user.Id
      };
      var comments = new List<Comment>{ comment };
      var response = new List<CommentDto>
      {
        new CommentDto
        {
          Id = comment.Id,
          Content = comment.Content,
          PostId = comment.PostId,
          UserId = comment.UserId
        }
      };
      // Claim
      var Claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(Claims, "TestAuth");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
      _commentRepositoryMock.Setup(r => r.GetCommentsByUserId(user.Id)).ReturnsAsync(comments);
      _mapperMock.Setup(m => m.Map<List<CommentDto>>(comments)).Returns(response);
      // Act
      var result = await _commentService.CommentsOfUser();
      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Lấy danh sách bình luận thành công!");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task EditComment_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email };
      var request = new UpdateCommentRequestDto
      {
        CommentId = 1,
        Content = "Updated comment",
        ImageUrl = null
      };
      var comment = new Comment
      {
        Id = request.CommentId,
        Content = "Test comment",
        PostId = 1,
        UserId = user.Id
      };
      var response = new CommentDto
      {
        Id = comment.Id,
        Content = request.Content,
        PostId = comment.PostId,
        UserId = comment.UserId
      };
      // Claim
      var Claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(Claims, "TestAuth");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
      _commentRepositoryMock.Setup(r => r.GetCommentById(request.CommentId)).ReturnsAsync(comment);
      _mapperMock.Setup(m => m.Map<Comment>(request)).Returns(comment);
      _mapperMock.Setup(m => m.Map<CommentDto>(comment)).Returns(response);
      // Act
      var result = await _commentService.EditComment(request);
      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be($"Bạn đã sửa bình luận bài viết {comment.PostId} thành công!");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteComment_Authorize_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email };
      var commentId = 1;
      var comment = new Comment
      {
        Id = commentId,
        Content = "Test comment",
        PostId = 1,
        UserId = user.Id
      };
      // Claim
      var Claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(Claims, "TestAuth");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
      _commentRepositoryMock.Setup(r => r.GetCommentById(commentId)).ReturnsAsync(comment);
      // Act
      var result = await _commentService.DeleteComment(commentId);
      // Assert
      result.Status.Should().Be(204);
      result.Message.Should().Be($"Bạn đã xóa bình luận bài viết {comment.PostId} thành công!");
      result.Data.Should().BeNull();
    }
  }
}
