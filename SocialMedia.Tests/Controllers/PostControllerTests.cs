using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Controllers;
using SocialMedia.Models.Domain.Enums;
using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.Post;
using SocialMedia.Models.Entities;
using SocialMedia.Services;

namespace SocialMedia.Tests.Controllers
{
  public class PostControllerTests
  {
    private readonly Mock<IPostService> _postServiceMock;
    private readonly PostController _postController;
    public PostControllerTests()
    {
      _postServiceMock = new Mock<IPostService>();

      _postController = new PostController(_postServiceMock.Object);
    }
    [Fact]
    public async Task CreatePost_ValidRequest_ReturnOk()
    {
      // Arrange
      var request = new AddPostRequestDto
      {
        Title = "Test Title",
        Content = "Test Content",
        ImageUrl = null,
        PostStatus = PostEnum.Public
      };
      var response = new ApiResponse<PostDto>(201, "Bạn đã tạo bài viết thành công!", new PostDto
      {
        Id = 1,
        Title = "Test Title",
        Content = "Test Content",
        ImageUrl = null,
        PostStatus = PostEnum.Public,
        UserId = 1
      });
      _postServiceMock.Setup(x => x.CreatePost(It.IsAny<AddPostRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _postController.CreatePost(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<PostDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(201);
      apiResponse.Message.Should().Be("Bạn đã tạo bài viết thành công!");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task UpdatePost_ValidRequest_ReturnOk()
    {
      // Arrange
      var post = new Post { Id = 1, Title = "Test Title", Content = "Test Content", ImageUrl = null, PostStatus = PostEnum.Public, UserId = 1 };
      var request = new UpdatePostRequestDto
      {
        Id = 1,
        Title = "Updated Title",
        Content = "Updated Content",
        ImageUrl = null,
        PostStatus = PostEnum.Public
      };
      var response = new ApiResponse<PostDto>(200, $"Chỉnh sửa bài viết {post.Id} thành công !", new PostDto
      {
        Id = 1,
        Title = "Updated Title",
        Content = "Updated Content",
        ImageUrl = null,
        PostStatus = PostEnum.Public,
        UserId = 1
      });
      _postServiceMock.Setup(x => x.UpdatePost(It.IsAny<UpdatePostRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _postController.UpdatePost(request);
      // Assert
      var objectResult = Assert.IsType<ObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<PostDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be($"Chỉnh sửa bài viết {post.Id} thành công !");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task GetPost_User_ReturnOk()
    {
      // Arrage
      var author = "Author";
      var response = new ApiResponse<List<PostDto>>(200, $"Lấy thành công danh sách bài viết của người dùng {author}!", new List<PostDto>
      {
        new PostDto
        {
          Id = 1,
          Title = "Test Title",
          Content = "Test Content",
          ImageUrl = null,
          PostStatus = PostEnum.Public,
          UserId = 1
        }
      });
      _postServiceMock.Setup(x => x.GetPost()).ReturnsAsync(response);
      // Act
      var result = await _postController.GetPost();
      // Assert
      var objectResult = Assert.IsType<ObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<List<PostDto>>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be($"Lấy thành công danh sách bài viết của người dùng {author}!");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task DeletePost_User_ReturnOk()
    {
      // Arrange
      var postId = 1;
      var response = new ApiResponse<string>(200, $"Bạn đã xoá bài viết {postId} thành công!", null);
      _postServiceMock.Setup(x => x.DeletePost(postId)).ReturnsAsync(response);
      // Act
      var result = await _postController.DeletePost(postId);
      // Assert
      var objectResult = Assert.IsType<ObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<string>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be($"Bạn đã xoá bài viết {postId} thành công!");
      apiResponse.Data.Should().BeNull();
    }
    [Fact]
    public async Task SearchPost_KeyWord_ReturnOk()
    {
      // Arrage
      var keyWord = "post";
      var response = new ApiResponse<List<PostDto>>(200, $"Từ khoá {keyWord} không có kết quả trùng hợp !", new List<PostDto>
      {
        new PostDto
        {
          Id = 1,
          Title = "Test Title",
          Content = "Test Content",
          ImageUrl = null,
          PostStatus = PostEnum.Public,
          UserId = 1
        }
      });
      _postServiceMock.Setup(x => x.SearchPost(keyWord)).ReturnsAsync(response);
      // Act
      var result = await _postController.SearchPost(keyWord);
      // Assert
      var objectResult = Assert.IsType<ObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<List<PostDto>>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be($"Từ khoá {keyWord} không có kết quả trùng hợp !");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task GetTimeLine_User_Return()
    {
      // Arrage
      var response = new ApiResponse<List<PostDto>>(200, "Lấy thành công danh sách Time Line của bạn!", new List<PostDto>
      {
        new PostDto
        {
          Id = 1,
          Title = "Test Title",
          Content = "Test Content",
          ImageUrl = null,
          PostStatus = PostEnum.Public,
          UserId = 1
        }
      });
      _postServiceMock.Setup(x => x.GetTimeLine()).ReturnsAsync(response);
      // Act
      var result = await _postController.GetTimeLine();
      // Assert
      var objectResult = Assert.IsType<ObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<List<PostDto>>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Lấy thành công danh sách Time Line của bạn!");
      apiResponse.Data.Should().NotBeNull();
    }
  }
}
