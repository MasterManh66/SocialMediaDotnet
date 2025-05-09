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
using SocialMedia.Models.Dto.User;
using SocialMedia.Services;

namespace SocialMedia.Tests.Controllers
{
  public class UserControllerTests
  {
    private readonly Mock<IUserService> _userServiceMock;
    private UserController _userController;
    public UserControllerTests()
    {
      _userServiceMock = new Mock<IUserService>();
      _userController = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task RegisterUser_ValidRequest_ReturnCreated()
    {
      // Arrange
      var request = new RegisterUserRequestDto
      {
        Email = "manh@gmail.com",
        Password = "123456"
      };
      var response = new ApiResponse<string>(201, "User registered successfully", null);

      _userServiceMock.Setup(x => x.RegisterUser(request)).ReturnsAsync(response);
      // Act
      var result = await _userController.RegisterUser(request);
      // Assert
      var objectResult = Assert.IsType<CreatedAtActionResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<string>>(objectResult.Value);

      apiResponse.Status.Should().Be(201);
      apiResponse.Message.Should().Be("User registered successfully");
      apiResponse.Data.Should().BeNull();
    }

    [Fact]
    public async Task LoginUser_ValidRequest_ReturnsSuccess()
    {
      // Arrange
      var request = new LoginUserRequestDto
      {
        Email = "manh@gmail.com",
        Password = "123456"
      };
      var otp = "246351"; 
      var response = new ApiResponse<LoginUserResponseDto>(
        200, "Đăng nhập thành công, vui lòng xác thực Otp", 
        new LoginUserResponseDto { Otp = otp }
      );

      _userServiceMock.Setup(x => x.LoginUser(It.IsAny<LoginUserRequestDto>())).ReturnsAsync(response);

      // Act
      var result = await _userController.LoginUser(request);

      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<LoginUserResponseDto>>(objectResult.Value);

      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Đăng nhập thành công, vui lòng xác thực Otp");

      apiResponse.Data.Should().NotBeNull();
      apiResponse.Data.Otp.Should().Be(otp);
    }

  }
}
