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
using SocialMedia.Models.Domain.Enums;
using SocialMedia.Models.Dto.Post;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using SocialMedia.Services;

namespace SocialMedia.Tests.Services
{
  public class PostServiceTests
  {
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFriendRepository> _friendRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly PostService _postService;

    public PostServiceTests()
    {
      _postRepositoryMock = new Mock<IPostRepository>();
      _imageServiceMock = new Mock<IImageService>();
      _userRepositoryMock = new Mock<IUserRepository>();
      _friendRepositoryMock = new Mock<IFriendRepository>();
      _mapperMock = new Mock<IMapper>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

      _postService = new PostService(_userRepositoryMock.Object, _postRepositoryMock.Object,
        _mapperMock.Object, _imageServiceMock.Object, _httpContextAccessorMock.Object,_friendRepositoryMock.Object
      );
    }

    [Fact]
    public async Task CreatePost_ValidRequest_ReturnSuccess()
    {
      // arrange
      var email = "manh@gmail.com";
      var title = "Create Post";
      var content = "Create a new post for user";
      var status = PostEnum.Public;

      var request = new AddPostRequestDto
      {
        Title = title,
        Content = content,
        ImageUrl = null,
        PostStatus = status
      };
      var user = new User 
      { 
        Id = 1,
        Email = email,
        FirstName = "Nguyen",
        LastName = "Manh",
      };
      string author = $"{user.FirstName} {user.LastName}";
      var post = new Post
      {
        Id = 1,
        Title = title,
        Content = content,
        ImageUrl = null,
        PostStatus = status,
        UserId = user.Id
      };
      var response = new PostDto
      {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        ImageUrl = null,
        PostStatus = post.PostStatus,
        UserId = user.Id
      };
      // claim
      var claim = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claim);
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
      _mapperMock.Setup(x => x.Map<Post>(request)).Returns(post);
      _mapperMock.Setup(x => x.Map<PostDto>(post)).Returns(response);
      // act
      var result = await _postService.CreatePost(request);  
      // assert
      result.Status.Should().Be(201);
      result.Message.Should().Be("Bạn đã tạo bài viết thành công!");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatePost_validRequest_ReturnSuccess()
    {
      // arrange
      var email = "manh@gmail.com";
      var id = 1;
      var title = "Update Post";
      var content = "Update a new post for user";
      var status = PostEnum.Public;
      var user = new User
      {
        Id = 1,
        Email = email,
      };
      var request = new UpdatePostRequestDto
      {
        Id = id,
        Title = title,
        Content = content,
        ImageUrl = null,
        PostStatus = status
      };
      var post = new Post
      {
        Id = id,
        Title = "Create Post",
        Content = "Create a new post for user",
        ImageUrl = null,
        PostStatus = status,
        UserId = user.Id
      };
      var response = new PostDto
      {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        ImageUrl = null,
        PostStatus = post.PostStatus,
        UserId = user.Id
      };
      // claim
      var claim = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claim);
      var claimsPrincipal = new ClaimsPrincipal(identity);
      
      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.GetPostById(id)).ReturnsAsync(post);
      _mapperMock.Setup(x => x.Map<Post>(request)).Returns(post);
      _mapperMock.Setup(x => x.Map<PostDto>(post)).Returns(response);
      // act
      var result = await _postService.UpdatePost(request);
      // assert
      result.Status.Should().Be(200);
      result.Message.Should().Be($"Chỉnh sửa bài viết {post.Id} thành công !");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPost_Authorize_ReturnSeccess()
    {
      // arrange
      var email = "manh@gmail.com";
      var user = new User
      {
        Id = 1,
        Email = email,
        FirstName = "Nguyen",
        LastName = "Manh",
      };
      string author = $"{user.FirstName} {user.LastName}";
      var post = new Post
      {
        Id = 1,
        Title = "Create Post",
        Content = "Create a new post for user",
        ImageUrl = null,
        PostStatus = PostEnum.Public,
        UserId = user.Id
      };
      var posts = new List<Post> { post };
      var response = new List<PostDto>
      {
        new PostDto
        {
          Id = post.Id,
          Title = post.Title,
          Content = post.Content,
          ImageUrl = null,
          PostStatus = post.PostStatus,
          UserId = user.Id
        }
      };
      // claim
      var claim = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claim);
      var claimsPrincipal = new ClaimsPrincipal(identity);
      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _userRepositoryMock.Setup(x => x.GetUserById(user.Id)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.GetPostsByUserId(user.Id)).ReturnsAsync(posts);
      _mapperMock.Setup(x => x.Map<List<PostDto>>(posts)).Returns(response);
      // act
      var result = await _postService.GetPost();
      // assert
      result.Status.Should().Be(200);
      result.Message.Should().Be($"Lấy thành công danh sách bài viết của người dùng {author}!");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task DeletePost_ValidPostId_ReturnSuccess()
    {
      // arrange
      var email = "manh@gmail.com";
      var user = new User
      {
        Id = 1,
        Email = email,
      };
      var post = new Post
      {
        Id = 1,
        Title = "Create Post",
        Content = "Create a new post for user",
        ImageUrl = null,
        PostStatus = PostEnum.Public,
        UserId = user.Id
      };
      // claim
      var claim = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claim);
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.GetPostById(post.Id)).ReturnsAsync(post);
      // act
      var result = await _postService.DeletePost(post.Id);
      // assert
      result.Status.Should().Be(200);
      result.Message.Should().Be($"Bạn đã xoá bài viết {post.Id} thành công!");
      result.Data.Should().BeNull();
    }

    [Fact]
    public async Task SearchPost_ValidKey_ReturnSuccess()
    {
      // arrange
      var email = "manh@gmail.com";
      var user = new User
      {
        Id = 1,
        Email = email
      };
      var post = new Post
      {
        Id = 1,
        Title = "Create Post",
        Content = "Create a new post for user",
        ImageUrl = null,
        PostStatus = PostEnum.Public,
        UserId = user.Id
      };
      var posts = new List<Post> { post };
      var keyWord = "Create";
      var response = new List<PostDto>
      {
        new PostDto
        {
          Id = post.Id,
          Title = post.Title,
          Content = post.Content,
          ImageUrl = null,
          PostStatus = post.PostStatus,
          UserId = user.Id
        }
      };
      // claim
      var claim = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claim);
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.SearchPostByKey(keyWord)).ReturnsAsync(posts);
      _mapperMock.Setup(x => x.Map<List<PostDto>>(posts)).Returns(response);
      // act
      var result = await _postService.SearchPost(keyWord);
      // assert
      result.Status.Should().Be(200);
      result.Message.Should().Be($"Tìm kiếm theo từ khoá {keyWord} thành công !");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTimeLine_Authorize_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User
      {
        Id = 1,
        Email = email,
      };

      var friend = new User
      {
        Id = 2,
        Email = "friend@gmail.com",
      };

      var friendship = new Friend
      {
        RequesterId = user.Id,
        ReceiverId = friend.Id,
        FriendStatus = FriendEnum.Accepted
      };

      var postUser = new Post
      {
        Id = 1,
        Title = "User Post",
        Content = "Post by user",
        PostStatus = PostEnum.Public,
        UserId = user.Id,
        CreatedAt = DateTime.UtcNow
      };

      var postFriend = new Post
      {
        Id = 2,
        Title = "Friend Post",
        Content = "Post by friend",
        PostStatus = PostEnum.Friends,
        UserId = friend.Id,
        CreatedAt = DateTime.UtcNow.AddMinutes(-1) 
      };

      var allPosts = new List<Post> { postUser, postFriend };

      var expectedDto = new List<PostDto>
      {
        new PostDto
        {
          Id = postUser.Id,
          Title = postUser.Title,
          Content = postUser.Content,
          PostStatus = postUser.PostStatus,
          UserId = postUser.UserId
        },
        new PostDto
        {
          Id = postFriend.Id,
          Title = postFriend.Title,
          Content = postFriend.Content,
          PostStatus = postFriend.PostStatus,
          UserId = postFriend.UserId
        }
      };

      // Claims
      var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
      var identity = new ClaimsIdentity(claims);
      var principal = new ClaimsPrincipal(identity);
      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

      // Mock repositories
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _postRepositoryMock.Setup(x => x.GetPostsByUserId(user.Id)).ReturnsAsync(new List<Post> { postUser });
      _friendRepositoryMock.Setup(x => x.GetFriendsByUserId(user.Id)).ReturnsAsync(new List<Friend> { friendship });
      _postRepositoryMock.Setup(x => x.GetPostsByUserIds(It.IsAny<List<int>>()))
                         .ReturnsAsync(new List<Post> { postFriend });
      _mapperMock.Setup(x => x.Map<List<PostDto>>(It.IsAny<List<Post>>())).Returns(expectedDto);

      // Act
      var result = await _postService.GetTimeLine();

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(200);
      result.Message.Should().Be("Lấy thành công danh sách Time Line của bạn!");
      result.Data.Should().NotBeNull();
      result.Data.Should().HaveCount(2);
      result.Data[0].Id.Should().Be(postUser.Id);
      result.Data[1].Id.Should().Be(postFriend.Id);
    }

  }
}
