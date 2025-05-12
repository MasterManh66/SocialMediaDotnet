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
using SocialMedia.Models.Dto.Comment;
using SocialMedia.Models.Entities;
using SocialMedia.Services;

namespace SocialMedia.Tests.Controllers
{
  public class CommentControllerTests
  {
    private readonly Mock<ICommentService> _commentServiceMock;
    private readonly CommentController _commentController;

    public CommentControllerTests()
    {
      _commentServiceMock = new Mock<ICommentService>();
      _commentController = new CommentController(_commentServiceMock.Object);
    }

    [Fact]
    public async Task CreateComment_ValidRequest_ReturnsOk()
    {
      // Arrange
      var request = new AddCommentRequestDto
      {
        Content = "Test comment",
        ImageUrl = null,
        PostId = 1
      };
      var post = new Post
      {
        Id = 1,
        Content = "Test post",
        UserId = 1
      };
      var response = new ApiResponse<CommentDto>(201, $"Bạn đã bình luận bài viết {post.Id} thành công!", new CommentDto
      {
        Id = 1,
        Content = "Test comment",
        ImageUrl = null,
        PostId = 1,
        UserId = 1
      });
      _commentServiceMock.Setup(x => x.CreateComment(request)).ReturnsAsync(response);
      // Act
      var result = await _commentController.CreateComment(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<CommentDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(201);
      apiResponse.Message.Should().Be($"Bạn đã bình luận bài viết {post.Id} thành công!");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task CommentsOfUser_User_ReturnOk()
    {
      // Arrange
      var response = new ApiResponse<List<CommentDto>>(200, "Lấy danh sách bình luận thành công!", new List<CommentDto>
      {
        new CommentDto
        {
          Id = 1,
          Content = "Test comment",
          ImageUrl = null,
          PostId = 1,
          UserId = 1
        }
      });
      _commentServiceMock.Setup(x => x.CommentsOfUser()).ReturnsAsync(response);
      // Act
      var result = await _commentController.CommentsOfUser();
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<List<CommentDto>>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Lấy danh sách bình luận thành công!");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task EditComment_ValidRequest_ReturnsOk()
    {
      // Arrange
      var request = new UpdateCommentRequestDto
      {
        CommentId = 1,
        Content = "New comment",
        ImageUrl = null
      };
      var comment = new Comment
      {
        Id = 1,
        Content = "Test comment",
        ImageUrl = null,
        PostId = 1,
        UserId = 1
      };
      var response = new ApiResponse<CommentDto>(200, $"Bạn đã sửa bình luận bài viết {comment.PostId} thành công!", new CommentDto
      {
        Id = 1,
        Content = "Updated comment",
        ImageUrl = null,
        PostId = 1,
        UserId = 1
      });
      _commentServiceMock.Setup(x => x.EditComment(request)).ReturnsAsync(response);
      // Act
      var result = await _commentController.EditComment(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<CommentDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be($"Bạn đã sửa bình luận bài viết {comment.PostId} thành công!");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task DeleteComment_User_ReturnOk()
    {
      // Arrange
      var commentId = 1;
      var comment = new Comment
      {
        Id = 1,
        Content = "Test comment",
        ImageUrl = null,
        PostId = 1,
        UserId = 1
      };
      var response = new ApiResponse<CommentDto>(204, $"Bạn đã xóa bình luận bài viết {comment.PostId} thành công!", null);
      _commentServiceMock.Setup(x => x.DeleteComment(commentId)).ReturnsAsync(response);
      // Act
      var result = await _commentController.DeleteComment(commentId);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<CommentDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(204);
      apiResponse.Message.Should().Be($"Bạn đã xóa bình luận bài viết {comment.PostId} thành công!");
      apiResponse.Data.Should().BeNull();
    }
  }
}
