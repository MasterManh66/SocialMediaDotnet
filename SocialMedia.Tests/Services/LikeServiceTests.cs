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
using SocialMedia.Models.Dto.Like;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using SocialMedia.Services;

namespace SocialMedia.Tests.Services
{
  public class LikeServiceTests
  {
    private readonly Mock<ILikeRepository> _likeRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly LikeService _likeService;

    public LikeServiceTests()
    {
      _likeRepositoryMock = new Mock<ILikeRepository>();
      _postRepositoryMock = new Mock<IPostRepository>();
      _userRepositoryMock = new Mock<IUserRepository>();
      _mapperMock = new Mock<IMapper>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

      _likeService = new LikeService(
        _httpContextAccessorMock.Object, _likeRepositoryMock.Object,
        _postRepositoryMock.Object, _userRepositoryMock.Object, _mapperMock.Object
      );
    }

    [Fact]
    public async Task CreateLike_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email, FirstName = "Nguyen", LastName = "Manh" };
      var post = new Post { Id = 1, UserId = user.Id, Title = "Test post" };
      var request = new AddLikeRequestDto { PostId = post.Id };
      var like = new Like
      {
        Id = 1,
        PostId = post.Id,
        UserId = user.Id,
        CreatedAt = DateTime.UtcNow
      };
      var author = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Anonymous";
      var response = new LikeDto
      {
        Id = like.Id,
        PostId = like.PostId,
        UserId = like.UserId,
        PostTitle = post.Title,
        CreatedAt = like.CreatedAt,
      };
      // Claim
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claims);
      var principal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.GetPostById(post.Id)).ReturnsAsync(post);
      _mapperMock.Setup(x => x.Map<Like>(request)).Returns(like);
      _mapperMock.Setup(x => x.Map<LikeDto>(like)).Returns(response);
      // Act
      var result = await _likeService.CreateLike(request);
      // Assert
      result.Status.Should().Be(201);
      result.Message.Should().Be($"Bạn đã thích bài viết {request.PostId} của tác giả {author} thành công!");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLike_Authorize_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email };
      var like = new Like
      {
        Id = 1,
        PostId = 1,
        UserId = user.Id,
        CreatedAt = DateTime.UtcNow
      };
      var likes = new List<Like> { like };
      var response = new List<LikeDto>
      {
        new LikeDto
        {
          Id = like.Id,
          PostId = like.PostId,
          UserId = like.UserId,
          CreatedAt = like.CreatedAt,
        }
      };
      // Claim
      var Claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(Claims);
      var principal = new ClaimsPrincipal(identity);
      
      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _likeRepositoryMock.Setup(x => x.GetLikesByUserId(user.Id)).ReturnsAsync(likes);
      _mapperMock.Setup(x => x.Map<List<LikeDto>>(likes)).Returns(response);
      // Act
      var result = await _likeService.LikeOfUser();
      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Danh sách bài viết đã thích của bạn");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task unLike_Authorize_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User { Id = 1, Email = email, FirstName = "Nguyen", LastName = "Manh" };
      var request = new AddLikeRequestDto { PostId = 1 };
      var post = new Post { Id = 1, UserId = user.Id, Title = "Test post" };
      var like = new Like
      {
        Id = 1,
        PostId = post.Id,
        UserId = user.Id,
        CreatedAt = DateTime.UtcNow
      };
      var author = post.User != null ? $"{post.User.FirstName} {post.User.LastName}" : "Anonymous";
      // Claim
      var Claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(Claims);
      var principal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.GetPostById(request.PostId)).ReturnsAsync(post);
      _likeRepositoryMock.Setup(x => x.GetLikeByUserIdAndPostId(request.PostId, user.Id)).ReturnsAsync(like);
      // Act
      var result = await _likeService.UnlikePost(request);
      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be($"Bạn đã huỷ like bài viết {post.Id} của {author} thành công!");
      result.Data.Should().BeNull();
    }
  }
}
