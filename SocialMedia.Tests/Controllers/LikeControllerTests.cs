using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SocialMedia.Controllers;
using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.Like;
using SocialMedia.Models.Entities;
using SocialMedia.Services;

namespace SocialMedia.Tests.Controllers
{
  public class LikeControllerTests
  {
    private readonly Mock<ILikeService> _likeServiceMock;
    private readonly LikeController _likeController;

    public LikeControllerTests()
    {
      _likeServiceMock = new Mock<ILikeService>();
      _likeController = new LikeController(_likeServiceMock.Object);
    }

    [Fact]
    public async Task CreateLike_ValidPost_ReturnOk()
    {
      // Arrage
      var request = new AddLikeRequestDto
      {
        PostId = 1
      };
      var author = "Author";
      var response = new ApiResponse<LikeDto>(201, $"Bạn đã thích bài viết {request.PostId} của tác giả {author} thành công!", new LikeDto
      {
        Id = 1, PostTitle = "New Post", PostId = 1, UserId = 1
      });
      _likeServiceMock.Setup(x => x.CreateLike(request)).ReturnsAsync(response);
      // Act
      var result = await _likeController.CreateLike(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<LikeDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(201);
      apiResponse.Message.Should().Be($"Bạn đã thích bài viết {request.PostId} của tác giả {author} thành công!");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task LikeOfUser_user_ReturnOk()
    {
      // Arrage
      var response = new ApiResponse<List<LikeDto>>(200, "Danh sách bài viết đã thích của bạn", new List<LikeDto>
      {
        new LikeDto
        {
          Id = 1,
          PostId = 1,
          PostTitle = "New Post",
          UserId = 1
        }
      });
      _likeServiceMock.Setup(x => x.LikeOfUser()).ReturnsAsync(response);
      // Act
      var result = await _likeController.LikeOfUser();
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<List<LikeDto>>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Danh sách bài viết đã thích của bạn");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task UnlikePost_User_ReturnOk()
    {
      // Arrage
      var postId = 1;
      var post = new Post
      {
        Id = postId,
      };
      var author = "Author";
      var response = new ApiResponse<string>(200, $"Bạn đã huỷ like bài viết {post.Id} của {author} thành công!", null);
      _likeServiceMock.Setup(x => x.UnlikePost(postId)).ReturnsAsync(response);
      // Act
      var result = await _likeController.UnlikePost(postId);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<string>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be($"Bạn đã huỷ like bài viết {post.Id} của {author} thành công!");
      apiResponse.Data.Should().BeNull();
    }
  }
}
