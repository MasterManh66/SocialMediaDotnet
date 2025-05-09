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
using SocialMedia.Models.Dto.Friend;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using SocialMedia.Services;

namespace SocialMedia.Tests.Services
{
  public class FriendServiceTests
  {
    private readonly Mock<IFriendRepository> _friendRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly FriendService _friendService;

    public FriendServiceTests()
    {
      _friendRepositoryMock = new Mock<IFriendRepository>();
      _userRepositoryMock = new Mock<IUserRepository>();
      _mapperMock = new Mock<IMapper>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

      _friendService = new FriendService(
        _httpContextAccessorMock.Object, _friendRepositoryMock.Object,
        _userRepositoryMock.Object, _mapperMock.Object
      );
    }

    [Fact]
    public async Task SendFriendRequest_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User
      {
        Id = 1,
        Email = email,
        FirstName = "Manh",
        LastName = "Nguyen"
      };
      var receiverId = 2;
      var receiverUser = new User
      {
        Id = receiverId,
        Email = "receiver@gmail.com",
        FirstName = "Friend",
        LastName = "User"
      };
      var friend = new Friend
      {
        Id = 1,
        RequesterId = user.Id,
        ReceiverId = receiverId,
        FriendStatus = FriendEnum.Pending
      };
      var response = new FriendDto
      {
        UserId = user.Id,
        FullName = user.FirstName + " " + user.LastName,
        Avatar = user.ImageUrl,
        Address = user.Address,
        Job = user.Job,
        Gender = user.Gender,
        FriendStatus = FriendEnum.Pending
      };

      var claims = new List<Claim> { new Claim(ClaimTypes.Email, email) };
      var identity = new ClaimsIdentity(claims);
      var principal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
      _userRepositoryMock.Setup(x => x.GetUserById(receiverId)).ReturnsAsync(receiverUser);
      _friendRepositoryMock.Setup(x => x.GetFriendsByUserId(user.Id)).ReturnsAsync(new List<Friend>());
      _mapperMock.Setup(x => x.Map<FriendDto>(It.IsAny<Friend>())).Returns(response);

      // Act
      var result = await _friendService.SendFriendRequest(receiverId);

      // Assert
      result.Should().NotBeNull();
      result.Status.Should().Be(201);
      result.Message.Should().Be($"Bạn đã gửi lời mời kết bạn cho người dùng {receiverId} thành công!");
      result.Data.Should().NotBeNull();
      result.Data.FriendStatus.Should().Be(FriendEnum.Pending);
    }

  }
}
