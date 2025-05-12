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
using SocialMedia.Models.Dto.Friend;
using SocialMedia.Services;

namespace SocialMedia.Tests.Controllers
{
  public class FriendControllerTests
  {
    private readonly Mock<IFriendService> _friendServiceMock;
    private readonly FriendController _friendController;

    public FriendControllerTests()
    {
      _friendServiceMock = new Mock<IFriendService>();
      _friendController = new FriendController(_friendServiceMock.Object);
    }

    [Fact]
    public async Task SendFriendRequest_ReceiverId_ReturnOk()
    {
      // Arrange
      int receiverId = 1;
      var response = new ApiResponse<FriendDto>(201, $"Bạn đã gửi lời mời kết bạn cho người dùng {receiverId} thành công!", new FriendDto
      {
        UserId = 1,
        FullName = "Name Friend",
        Avatar = "02622f4a-b316-43a7-aee4-d42a03f35c08.jpg",
        Address = "Nam Dinh",
        Job = "Dev",
        Gender = GenderEnum.Male,
        FriendStatus = FriendEnum.Pending
      });
      _friendServiceMock.Setup(x => x.SendFriendRequest(receiverId)).ReturnsAsync(response);
      // Act
      var result = await _friendController.SendFriendRequest(receiverId);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<FriendDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(201);
      apiResponse.Message.Should().Be($"Bạn đã gửi lời mời kết bạn cho người dùng {receiverId} thành công!");
      apiResponse.Data.Should().NotBeNull();
    }
  }
}
