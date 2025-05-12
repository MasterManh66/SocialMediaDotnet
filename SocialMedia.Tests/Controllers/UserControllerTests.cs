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
    [Fact]
    public async Task VerifyOtp_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var token = "token";

      var request = new AuthUserRequestDto
      {
        Email = "manh@gmail.com",
        Otp = "246351"
      };
      var response = new ApiResponse<AuthUserResponseDto>(200, "Xác thực Otp thành công!", new AuthUserResponseDto { Token = token });
      _userServiceMock.Setup(x => x.VerifyOtp(It.IsAny<AuthUserRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _userController.VerifyOtp(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<AuthUserResponseDto>>(objectResult.Value);

      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Xác thực Otp thành công!");
      apiResponse.Data.Should().NotBeNull();
      apiResponse.Data.Token.Should().Be(token);
    }
    [Fact]
    public async Task ForgetPassword_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var request = new ForgetPasswordRequestDto { Email = "manh@gmail.com " };
      var otp = "246351";

      var response = new ApiResponse<ForgetPasswordResponseDto>(200, 
                  "Quên mật khẩu thành công, Vui lòng xác thực mã OTP để đổi mật khẩu mới!", new ForgetPasswordResponseDto { Otp = otp });
      _userServiceMock.Setup(x => x.ForgetPassword(It.IsAny<ForgetPasswordRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _userController.ForgetPassword(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<ForgetPasswordResponseDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Quên mật khẩu thành công, Vui lòng xác thực mã OTP để đổi mật khẩu mới!");
      apiResponse.Data.Should().NotBeNull();
      apiResponse.Data.Otp.Should().Be(otp);
    }
    [Fact]
    public async Task VerifyForgetPassword_ValidOtp_ReturnSuccess()
    {
      // Arrange
      var request = new VerifyForgetPasswordRequestDto
      {
        Email = "manh@gmail/com",
        Otp = "246351"
      };
      var link = "https://socialmedia.com/reset-password";
      var token = "token";
      var response = new ApiResponse<VerifyForgetPasswordResponseDto>(200, "Xác thực Otp quên mật khẩu thành công, vui lòng thay đổi mật khẩu mới!"
                                  , new VerifyForgetPasswordResponseDto { Link = link, Token = token });
      _userServiceMock.Setup(x => x.VerifyForgetPasword(It.IsAny<VerifyForgetPasswordRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _userController.VerifyForgetPassword(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<VerifyForgetPasswordResponseDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Xác thực Otp quên mật khẩu thành công, vui lòng thay đổi mật khẩu mới!");
      apiResponse.Data.Should().NotBeNull();
      apiResponse.Data.Link.Should().Be(link);
      apiResponse.Data.Token.Should().Be(token);
    }
    [Fact]
    public async Task ChangePassword_ValidPassword_ReturnSuccess()
    {
      // Arrange
      var request = new ChangePasswordRequestDto
      {
        NewPassword = "123456",
        ConfirmPassword = "123456"
      };
      var response = new ApiResponse<string>(200, "Mật khẩu đã được thay đổi thành công", null);
      _userServiceMock.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _userController.ChangePassword(request); 
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<string>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Mật khẩu đã được thay đổi thành công");
      apiResponse.Data.Should().BeNull();
    }
    [Fact]
    public async Task GetUser_User_ReturnSuccess()
    {
      // Arrange
      var response = new ApiResponse<UserDto>(200, "Lấy thành công thông tin của bạn", new UserDto
      {
        Id = 1, FirstName = "Manh", LastName = "Tran", Email = "manh@gmail.com",
        Address = "Ha Noi", Job = "Dev", ImageUrl = "0c9b4fbe-3f12-4ccf-8347-450ccb7713db.jpg",
        DateOfBirth = new DateOnly(2002,04, 06), Gender = GenderEnum.Male
      });
      _userServiceMock.Setup(x => x.GetUser()).ReturnsAsync(response);
      // Act
      var result = await _userController.GetUser();
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<UserDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Lấy thành công thông tin của bạn");
      apiResponse.Data.Should().NotBeNull();
    }
    [Fact]
    public async Task UpdateUser_ValidRequest_ReturnSuccess()
    {
      // Arrange
      var request = new UpdateUserRequestDto
      {
        FirstName = "Manh",
        LastName = "Tran",
        Address = "Ha Noi",
        Job = "Dev",
        ImageUrl = null,
        DateOfBirth = new DateOnly(2002, 04, 06),
        Gender = GenderEnum.Male
      };
      var response = new ApiResponse<UserDto>(200, "Cập nhật thành công thông tin của bạn!", new UserDto
      {
        FirstName = "Manh",
        LastName = "Tran",
        Address = "Ha Noi",
        Job = "Dev",
        ImageUrl = null,
        DateOfBirth = new DateOnly(2002, 04, 06),
        Gender = GenderEnum.Male
      });
      _userServiceMock.Setup(x => x.UpdateUser(It.IsAny<UpdateUserRequestDto>())).ReturnsAsync(response);
      // Act
      var result = await _userController.UpdateUser(request);
      // Assert
      var objectResult = Assert.IsType<OkObjectResult>(result);
      var apiResponse = Assert.IsType<ApiResponse<UserDto>>(objectResult.Value);
      apiResponse.Status.Should().Be(200);
      apiResponse.Message.Should().Be("Cập nhật thành công thông tin của bạn!");
      apiResponse.Data.Should().NotBeNull();
    }
  }
}
