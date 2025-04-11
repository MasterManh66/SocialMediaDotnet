using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using BCrypt.Net;
using System.Security.Cryptography;
using SocialMedia.Models.Dto.Request;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SocialMedia.Services
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly RedisService _redisService;
    private readonly IJwtService _jwtService;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, RedisService redisService, IJwtService jwtService)
    {
      _userRepository = userRepository;
      _roleRepository = roleRepository;
      _redisService = redisService;
      _jwtService = jwtService;
    }

    public async Task<ApiResponse<string>> RegisterUser(RegisterUserRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      {
        return new ApiResponse<string>(400, "Email hoặc password chỉ là khoẳng trắng", null);
      }
      //Check user already exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user != null)
      {
        return new ApiResponse<string>(409, "Người dùng đã tồn tại!", null);
      }
      //check Role User
      var role = await _roleRepository.GetRoleByNameAsync("User");
      if (role == null)
      {
        return new ApiResponse<string>(404, "Role User not found", null);
      }
      //Create new user
      string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
      var newUser = new User
      {
        Email = request.Email.ToLower(),
        Password = hashedPassword,
        UserRoles = new List<UserRole>
         {
             new UserRole
             {
                 RoleId = role.Id
             }
         }
      };
      //Add user to database
      await _userRepository.AddUserAsync(newUser);
      await _userRepository.SaveChangesAsync();

      return new ApiResponse<string>(201, "Bạn đã tạo tài khoản mới thành công!", newUser.Email);
    }

    public async Task<ApiResponse<LoginResponse>> LoginUser(LoginUserRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      {
        return new ApiResponse<LoginResponse>(400, "Email hoặc password đang là khoảng trống", null);
      }
      //check user exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<LoginResponse>(404, "Người dùng không tồn tại", null);
      }
      //check password
      bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
      if (!isPasswordValid)
      {
        return new ApiResponse<LoginResponse>(400, "Bạn đã nhập sai Password!", null);
      }
      //create otp
      int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
      await _redisService.SetValueAsync($"otp:{user.Email.ToLower()}", otp.ToString(), TimeSpan.FromMinutes(5));
      return new ApiResponse<LoginResponse>(200, "Đăng nhập thành công, vui lòng xác thực Otp", new LoginResponse
      {
        Otp = otp.ToString()
      });
    }

    public async Task<ApiResponse<AuthResponse>> VerifyOtp(AuthRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
      {
        return new ApiResponse<AuthResponse>(400, "Otp hoặc Email chỉ là khoảng trắng", null);
      }
      //check otp & email exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<AuthResponse>(404, "User không tồn tại", null);
      }
      var otp = await _redisService.GetValueAsync($"otp:{request.Email.ToLower()}");
      if ( request.Otp != otp )
      {
        return new ApiResponse<AuthResponse>(404, "Otp nhập không đúng!", null);
      }
      //Create token
      var token = _jwtService.GenerateToken(user.Id, user.Email);
      return new ApiResponse<AuthResponse>(200, "Xác thực Otp thành công!", new AuthResponse
      {
        Token = token
      });
    }

    public async Task<ApiResponse<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email))
      {
        return new ApiResponse<ForgetPasswordResponse>(400, "Email hoặc đang là khoảng trống", null);
      }
      //check user exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<ForgetPasswordResponse>(404, "Người dùng không tồn tại", null);
      }
      //create otp
      int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
      await _redisService.SetValueAsync($"otp:{user.Email.ToLower()}", otp.ToString(), TimeSpan.FromMinutes(5));
      return new ApiResponse<ForgetPasswordResponse>(200, "Quên mật khẩu thành công, Vui lòng xác thực mã OTP để đổi mật khẩu mới!", new ForgetPasswordResponse
      {
        Otp = otp.ToString()
      });
    }

    public async Task<ApiResponse<VerifyForgetPasswordResponse>> VerifyForgetPasword(VerifyForgetPasswordRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
      {
        return new ApiResponse<VerifyForgetPasswordResponse>(400, "Email & Otp đang là khoảng trống", null);
      }
      //check email & otp exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<VerifyForgetPasswordResponse>(404, "Người dùng không tồn tại", null);
      }
      var otp = await _redisService.GetValueAsync($"otp:{request.Email.ToLower()}");
      if (request.Otp != otp)
      {
        return new ApiResponse<VerifyForgetPasswordResponse>(404, "Otp không đúng!", null);
      }
      string token = _jwtService.GenerateToken(user.Id, user.Email);
      return new ApiResponse<VerifyForgetPasswordResponse>(200, "Xác thực Otp quên mật khẩu thành công, vui lòng thay đổi mật khẩu mới!", new VerifyForgetPasswordResponse
      {
        Link =$"https://localhost:5082/User/ChangePassword?token={token}",
        Token = token
      });
    }

    //public async Task<ApiResponse<string>> ChangePassword(ChangePasswordRequest request)
    //{
    //  //check request
    //  if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
    //  {
    //    return new ApiResponse<string>(400, "Mật khẩu mới hoặc xác nhận mật khẩu đang là khoảng trống", null);
    //  }
    //  if (request.NewPassword != request.ConfirmPassword)
    //  {
    //    return new ApiResponse<string>(400, "Mật khẩu xác nhận không khớp", null);
    //  }
    //  //check token
    //  var claimsPrincipal = _jwtService.ValidateToken(token);
    //  if (claimsPrincipal == null)
    //  {
    //    return new ApiResponse<string>(401, "Token không hợp lệ!", null);
    //  }

    //  // Lấy UserId từ token
    //  var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //  if (string.IsNullOrEmpty(userId))
    //  {
    //    return new ApiResponse<string>(401, "Không tìm thấy ID người dùng trong token!", null);
    //  }

    //  // Tìm user trong database (giả sử có UserService)
    //  var user = await _userService.GetUserByIdAsync(userId);
    //  if (user == null)
    //  {
    //    return new ApiResponse<string>(404, "Người dùng không tồn tại!", null);
    //  }

    //  // Cập nhật mật khẩu mới
    //  user = _passwordHasher.HashPassword(request.NewPassword);
    //  await _userService.UpdateUserAsync(user);

    //  return new ApiResponse<string>(200, "Mật khẩu đã được thay đổi thành công", null);
    //}
  }
}
