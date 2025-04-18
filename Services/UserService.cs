﻿using SocialMedia.Models.Dto.Response;
using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using BCrypt.Net;
using System.Security.Cryptography;
using SocialMedia.Models.Dto.Request;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ClosedXML.Excel;

namespace SocialMedia.Services
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IFriendRepository _friendRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly RedisService _redisService;
    private readonly IJwtService _jwtService;
    private readonly IImageService _imageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, RedisService redisService,
                      IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IImageService imageService,
                      IPostRepository postRepository, ICommentRepository commentRepository, ILikeRepository likeRepository, IFriendRepository friendRepository)
    {
      _userRepository = userRepository;
      _commentRepository = commentRepository;
      _likeRepository = likeRepository;
      _postRepository = postRepository;
      _friendRepository = friendRepository;
      _roleRepository = roleRepository;
      _redisService = redisService;
      _jwtService = jwtService;
      _httpContextAccessor = httpContextAccessor;
      _imageService = imageService;
    }
    public string? GetCurrentUserEmail()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    }
    public Task<User?> GetUserByEmailAsync(string email)
    {
      return _userRepository.GetByEmailAsync(email);
    }

    public async Task<ApiResponse<string>> RegisterUser(RegisterUserRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      {
        return new ApiResponse<string>(400, "Email hoặc password chỉ là khoảng trắng", null);
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
        return new ApiResponse<string>(404, "Role User không tồn tại!", null);
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

    public async Task<ApiResponse<string>> ChangePassword(ChangePasswordRequest request)
    {
      //check request
      if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
      {
        return new ApiResponse<string>(400, "Mật khẩu mới hoặc xác nhận mật khẩu đang là khoảng trống", null);
      }
      if (request.NewPassword != request.ConfirmPassword)
      {
        return new ApiResponse<string>(400, "Mật khẩu xác nhận không khớp", null);
      }
      //get user email from token
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<string>(401, "Không thể xác thực người dùng!", null);
      }
      //find user by email
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<string>(404, "Người dùng không tồn tại!", null);
      }
      //hash password
      user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
      //update DB
      await _userRepository.UpdateAsync(user);
      return new ApiResponse<string>(200, "Mật khẩu đã được thay đổi thành công", null);
    }

    public async Task<ApiResponse<UserResponse>> GetUser()
    {
      //check token
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<UserResponse>(401, "Không thể xác thực người dùng!", null);
      }
      //get user by email
      var user = await GetUserByEmailAsync(email);
      if (user == null)
      {
        return new ApiResponse<UserResponse>(404, "Người dùng không tồn tại!", null);
      }
      return new ApiResponse<UserResponse>(200, "Lấy thành công thông tin của bạn", new UserResponse
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FullName = user.FirstName + " " + user.LastName,
        Email = user.Email,
        Address = user.Address,
        Job = user.Job,
        ImageUrl = user.ImageUrl,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender
      });
    }

    public async Task<ApiResponse<UserResponse>> UpdateUser(UserRequest request)
    {
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<UserResponse>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      //check user
      var enail = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(enail))
      {
        return new ApiResponse<UserResponse>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(enail);
      if (user == null)
      {
        return new ApiResponse<UserResponse>(404, "Người dùng không tồn tại!", null);
      }
      //update user
      bool isUpdated = false;
      if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != user.FirstName)
      {
        user.FirstName = request.FirstName;
        isUpdated = true;
      }
      if (!string.IsNullOrEmpty(request.LastName) && request.LastName != user.LastName)
      {
        user.LastName = request.LastName;
        isUpdated = true;
      }
      if (!string.IsNullOrEmpty(request.Address) && request.Address != user.Address)
      {
        user.Address = request.Address;
        isUpdated = true;
      }
      if (!string.IsNullOrEmpty(request.Job) && request.Job != user.Job)
      {
        user.Job = request.Job;
        isUpdated = true;
      }
      if (!request.DateOfBirth.Equals(default) && request.DateOfBirth != user.DateOfBirth)
      {
        user.DateOfBirth = request.DateOfBirth;
        isUpdated = true;
      }
      if (request.Gender != user.Gender)
      {
        user.Gender = request.Gender;
        isUpdated = true;
      }
      if (!string.IsNullOrEmpty(imageUrl) && imageUrl != user.ImageUrl)
      {
        user.ImageUrl = imageUrl;
        isUpdated = true;
      }
      if (!isUpdated)
      {
        return new ApiResponse<UserResponse>(400, "Không có thông tin nào để cập nhật!", null);
      } else
      {
        await _userRepository.UpdateAsync(user);
      }
      return new ApiResponse<UserResponse>(200, "Cập nhật thành công thông tin của bạn!", new UserResponse
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FullName = user.FirstName + "" + user.LastName,
        Email = user.Email,
        Address = user.Address,
        Job = user.Job,
        ImageUrl = user.ImageUrl,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender,
      });
    }

    public async Task<List<UserReportResponse>> ReportOfUser()
    {
      //check user
      var enail = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(enail))
      {
        return new List<UserReportResponse>();
      }
      var user = await GetUserByEmailAsync(enail);
      if (user == null)
      {
        return new List<UserReportResponse>();
      }
      var users = await _userRepository.GetUserById(user.Id);
      if (users == null)
      {
        return new List<UserReportResponse>();
      }
      //config Date and report
      var startDate = DateTime.UtcNow.AddDays(-7);
      var endDate = DateTime.UtcNow;
      var report = new List<UserReportResponse>();
      //export report
      int postCount = await _postRepository.CountPostsByUserId(users.Id, startDate, endDate);
      int commentCount = await _commentRepository.CountCommentsByUserId(users.Id, startDate, endDate);
      int likeCount = await _likeRepository.CountLikeByUserId(users.Id, startDate, endDate);
      int friendCount = await _friendRepository.CountFriendByUserId(users.Id, startDate, endDate);
      //response
      report.Add(new UserReportResponse 
      {
        Id = users.Id,
        FullName = $"{users.FirstName} {users.LastName}",
        Email = user.Email,
        TotalPosts = postCount,
        TotalComments = commentCount,
        TotalLikes = likeCount,
        NewFriends = friendCount
      });
      return report;
    }
    public byte[] ExportUserReportsToExcel(List<UserReportResponse> reports)
    {
      var startDate = DateTime.UtcNow.AddDays(-7).ToString("dd/MM/yyyy");
      var endDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
      using var workbook = new XLWorkbook();
      var worksheet = workbook.Worksheets.Add("User Report");
      //Title
      worksheet.Cell(1, 1).Value = $"Báo cáo hoạt động của bạn từ ngày {startDate} đến ngày {endDate}";
      worksheet.Range(1, 1, 1, 7).Merge();
      worksheet.Cell(1, 1).Style.Font.Bold = true;
      worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      //Content
      worksheet.Cell(2, 1).Value = "User ID";
      worksheet.Cell(2, 2).Value = "Full Name";
      worksheet.Cell(2, 3).Value = "Email";
      worksheet.Cell(2, 4).Value = "Posts";
      worksheet.Cell(2, 5).Value = "Likes";
      worksheet.Cell(2, 6).Value = "Comments";
      worksheet.Cell(2, 7).Value = "New Friends";

      for (int i = 0; i < reports.Count; i++)
      {
        var r = reports[i];
        worksheet.Cell(i + 3, 1).Value = r.Id;
        worksheet.Cell(i + 3, 2).Value = r.FullName;
        worksheet.Cell(i + 3, 3).Value = r.Email;
        worksheet.Cell(i + 3, 4).Value = r.TotalPosts;
        worksheet.Cell(i + 3, 5).Value = r.TotalLikes;
        worksheet.Cell(i + 3, 6).Value = r.TotalComments;
        worksheet.Cell(i + 3, 7).Value = r.NewFriends;
      }
      worksheet.Columns().AdjustToContents();
      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      return stream.ToArray();
    }
  }
}
