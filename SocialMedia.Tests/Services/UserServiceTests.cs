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
using SocialMedia.Models.Dto.User;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using SocialMedia.Services;
using SocialMedia.Models.Domain.Enums;
using System.Net;
using System.IO;

namespace SocialMedia.Tests.Services
{
  public class UserServiceTests
  {
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IImageService> _imageServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ILikeRepository> _likeRepositoryMock;
    private readonly Mock<IFriendRepository> _friendRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
      _userRepositoryMock = new Mock<IUserRepository>();
      _roleRepositoryMock = new Mock<IRoleRepository>();
      _redisServiceMock = new Mock<IRedisService>();
      _jwtServiceMock = new Mock<IJwtService>();
      _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
      _imageServiceMock = new Mock<IImageService>();
      _mapperMock = new Mock<IMapper>();
      _postRepositoryMock = new Mock<IPostRepository>();
      _commentRepositoryMock = new Mock<ICommentRepository>();
      _likeRepositoryMock = new Mock<ILikeRepository>();
      _friendRepositoryMock = new Mock<IFriendRepository>();

      _userService = new UserService(
        _userRepositoryMock.Object, _roleRepositoryMock.Object, _redisServiceMock.Object,
        _jwtServiceMock.Object, _httpContextAccessorMock.Object, _imageServiceMock.Object,
        _mapperMock.Object, _postRepositoryMock.Object, _commentRepositoryMock.Object,
        _likeRepositoryMock.Object, _friendRepositoryMock.Object
      );
    }
    [Fact]
    public async Task RegisterUser_RoleNotFound_ReturnsNotFoundResponse()
    {
      // Arrange
      var request = new RegisterUserRequestDto
      {
        Email = "manh@gmail.com",
        Password = "123456"
      };

      _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("User"));

      // Act
      var result = await _userService.RegisterUser(request);

      // Assert
      result.Status.Should().Be(404);
      result.Message.Should().Be("Role User không tồn tại!");
      result.Data.Should().BeNull();
    }

    [Fact]
    public async Task ResgisterUser_ValidRequest_ReturnsCreatedResponse()
    {
      // Arrange
      var request = new RegisterUserRequestDto
      {
        Email = "manh@gmail.com",
        Password = "123456"
      };
      var role = new Role
      {
        Id = 1,
        RoleName = "User"
      };

      _roleRepositoryMock.Setup(r => r.GetRoleByNameAsync("User")).ReturnsAsync(role);
      _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

      // Act
      var result = await _userService.RegisterUser(request);

      // Assert
      result.Status.Should().Be(201);
      result.Message.Should().Be("Bạn đã tạo tài khoản mới thành công!");
      result.Data.Should().Be("manh@gmail.com");
    }

    [Fact]
    public async Task LoginUser_ValidRequest_ReturnsLoginSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var password = "123456";
      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
      var user = new User { Email = email.ToLower(), Password = hashedPassword };

      _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email.ToLower())).ReturnsAsync(user);
      _redisServiceMock.Setup(r => r.SetValueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);

      // Act
      var request = new LoginUserRequestDto
      { 
        Email = email, 
        Password = password 
      };
      var result = await _userService.LoginUser(request);

      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Đăng nhập thành công, vui lòng xác thực Otp");
      result.Data.Should().NotBeNull();
      result.Data.Otp.Should().NotBeNullOrEmpty();
      result.Data.Otp.Should().MatchRegex(@"^\d{6}$");
    }

    [Fact]
    public async Task VerifyOtp_ValidOtp_ResturnsVerifySuccess()
    {
      //  Arrange
      var email = "manh@gmail.com";
      var otp = "245368";
      var user = new User
      {
        Id = 1,
        Email = email.ToLower()
      };

      _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email.ToLower())).ReturnsAsync(user);
      _redisServiceMock.Setup(r => r.GetValueAsync($"otp:{email.ToLower()}")).ReturnsAsync(otp);
      _jwtServiceMock.Setup(r => r.GenerateToken(user.Id, email.ToLower())).Returns("token");

      // Act
      var request = new AuthUserRequestDto
      {
        Email = email,
        Otp = otp
      };
      var result = await _userService.VerifyOtp(request);

      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Xác thực Otp thành công!");
      result.Data.Should().NotBeNull();
      result.Data.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ForgetPassword_ValidPassword_ReturnsOtp()
    {
      // Arrange
      var email = "manh@gmail.com";
      var request = new ForgetPasswordRequestDto
      {
        Email = email
      };

      var user = new User
      {
        Id = 1,
        Email = email.ToLower()
      };

      _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email.ToLower())).ReturnsAsync(user);
      _redisServiceMock.Setup(r => r.SetValueAsync(email.ToLower(), It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);

      // Act
      var result = await _userService.ForgetPassword(request);

      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Quên mật khẩu thành công, Vui lòng xác thực mã OTP để đổi mật khẩu mới!");
      result.Data.Should().NotBeNull();
      result.Data.Otp.Should().NotBeNullOrEmpty();
      result.Data.Otp.Should().MatchRegex(@"^\d{6}$");
    }

    [Fact]
    public async Task VerifyForgetPassword_ValidOtp_ReturnsToken()
    {
      // Arrange
      var email = "manh@gmail.com";
      var otp = "245368";
      var request = new VerifyForgetPasswordRequestDto
      {
        Email = email,
        Otp = otp
      };
      var user = new User
      {
        Id = 1,
        Email = email.ToLower()
      };

      _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(email.ToLower())).ReturnsAsync(user);
      _jwtServiceMock.Setup(r => r.GenerateToken(user.Id, email.ToLower())).Returns("token");
      _redisServiceMock.Setup(r => r.GetValueAsync($"otp:{email.ToLower()}")).ReturnsAsync(otp);

      // Act
      var result = await _userService.VerifyForgetPasword(request);

      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Xác thực Otp quên mật khẩu thành công, vui lòng thay đổi mật khẩu mới!");
      result.Data.Should().NotBeNull();
      result.Data.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ChangePassword_ValidRequest_ReturnsSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var newPassword = "123456";
      var confirmPassword = "123456";

      var request = new ChangePasswordRequestDto
      {
        NewPassword = newPassword,
        ConfirmPassword = confirmPassword
      };

      var user = new User
      {
        Id = 1,
        Email = email.ToLower()
      };

      // Mock claim
      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email)
      };
      var identity = new ClaimsIdentity(claims, "token");
      var claimsPrincipal = new ClaimsPrincipal(identity);
      var httpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      };
      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
      user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email.ToLower())).ReturnsAsync(user);
      _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

      // Act
      var result = await _userService.ChangePassword(request);

      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Mật khẩu đã được thay đổi thành công");
      result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetUser_ReturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var user = new User
      {
        Id = 1,
        Email = email.ToLower(),
        FirstName = "Nguyen",
        LastName = "Manh",
        Address = "Ha Noi",
        Job = "Developer",
        ImageUrl = "4c1020f1-3065-41d9-9b7c-e0bb1f35a7af.jpg",
        DateOfBirth = new DateOnly(2002, 3, 10),
        Gender = GenderEnum.Male
      };

      var userDto = new UserDto
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Address = user.Address,
        Job = user.Job,
        ImageUrl = user.ImageUrl,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender
      };

      var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email.ToLower()) 
      };
      var identity = new ClaimsIdentity(claims, "token");
      var claimsPrincipal = new ClaimsPrincipal(identity);
      var httpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      };
      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

      _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

      _userRepositoryMock.Setup(r => r.GetByEmailAsync(email.ToLower())).ReturnsAsync(user);

      // Act
      var result = await _userService.GetUser();

      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Lấy thành công thông tin của bạn");
      result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateUser_ValidRequest_ResturnSuccess()
    {
      // Arrange
      var email = "manh@gmail.com";
      var firstName = "Nguyen";
      var lastName = "Manh";
      var address = "Ha Noi";
      var job = "Developer";
      var dateOfBirth = new DateOnly(2002, 3, 10);
      var gender = GenderEnum.Male;

      var user = new User
      {
        Id = 1,
        Email = email.ToLower(),
        FirstName = firstName,
        LastName = lastName,
        Address = address,
        Job = job,
        ImageUrl = "4c1020f1-3065-41d9-9b7c-e0bb1f35a7af.jpg",
        DateOfBirth = dateOfBirth,
        Gender = gender
      };

      var request = new UpdateUserRequestDto
      {
        FirstName = "Tran",
        LastName = lastName,
        Address = address,
        Job = "DevOps",
        ImageUrl = null,
        DateOfBirth = dateOfBirth,
        Gender = gender,
      };

      var userDto = new UserDto
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Address = user.Address,
        Job = user.Job,
        ImageUrl = user.ImageUrl,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender
      };
      // Mock claim
      var claim = new List<Claim>
      {
        new Claim(ClaimTypes.Email, email.ToLower())
      };
      var identity = new ClaimsIdentity(claim, "token");
      var claimsPrincipal = new ClaimsPrincipal(identity);

      _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext{ User = claimsPrincipal });
      _userRepositoryMock.Setup(x => x.GetByEmailAsync(email.ToLower())).ReturnsAsync(user);

      _mapperMock.Setup(m => m.Map<User>(request)).Returns(user);
      _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
      // Act
      var result = await _userService.UpdateUser(request);
      // Assert
      result.Status.Should().Be(200);
      result.Message.Should().Be("Cập nhật thành công thông tin của bạn!");
      result.Data.Should().NotBeNull();
    }
  }
}
