using SocialMedia.Models.Entities;
using SocialMedia.Repositories;
using BCrypt.Net;
using System.Security.Cryptography;
using SocialMedia.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ClosedXML.Excel;
using SocialMedia.Models.Dto.User;
using AutoMapper;
using Azure;

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
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, RedisService redisService,
                      IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IImageService imageService, IMapper mapper,
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
      _mapper = mapper;
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

    public async Task<ApiResponse<string>> RegisterUser(RegisterUserRequestDto request)
    {
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
      await _userRepository.AddAsync(newUser);

      return new ApiResponse<string>(201, "Bạn đã tạo tài khoản mới thành công!", newUser.Email);
    }

    public async Task<ApiResponse<LoginUserResponseDto>> LoginUser(LoginUserRequestDto request)
    {
      //check user exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<LoginUserResponseDto>(404, "Người dùng không tồn tại", null);
      }
      //check password
      bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
      if (!isPasswordValid)
      {
        return new ApiResponse<LoginUserResponseDto>(400, "Bạn đã nhập sai Password!", null);
      }
      //create otp
      int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
      await _redisService.SetValueAsync($"otp:{user.Email.ToLower()}", otp.ToString(), TimeSpan.FromMinutes(5));
      return new ApiResponse<LoginUserResponseDto>(200, "Đăng nhập thành công, vui lòng xác thực Otp", new LoginUserResponseDto
      {
        Otp = otp.ToString()
      });
    }

    public async Task<ApiResponse<AuthUserResponseDto>> VerifyOtp(AuthUserRequestDto request)
    {
      //check otp & email exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<AuthUserResponseDto>(404, "User không tồn tại", null);
      }
      var otp = await _redisService.GetValueAsync($"otp:{request.Email.ToLower()}");
      if ( request.Otp != otp )
      {
        return new ApiResponse<AuthUserResponseDto>(404, "Otp nhập không đúng!", null);
      }
      //Create token
      var token = _jwtService.GenerateToken(user.Id, user.Email);
      return new ApiResponse<AuthUserResponseDto>(200, "Xác thực Otp thành công!", new AuthUserResponseDto
      {
        Token = token
      });
    }

    public async Task<ApiResponse<ForgetPasswordResponseDto>> ForgetPassword(ForgetPasswordRequestDto request)
    {
      //check user exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<ForgetPasswordResponseDto>(404, "Người dùng không tồn tại", null);
      }
      //create otp
      int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
      await _redisService.SetValueAsync($"otp:{user.Email.ToLower()}", otp.ToString(), TimeSpan.FromMinutes(5));
      return new ApiResponse<ForgetPasswordResponseDto>(200, "Quên mật khẩu thành công, Vui lòng xác thực mã OTP để đổi mật khẩu mới!", new ForgetPasswordResponseDto
      {
        Otp = otp.ToString()
      });
    }

    public async Task<ApiResponse<VerifyForgetPasswordResponseDto>> VerifyForgetPasword(VerifyForgetPasswordRequestDto request)
    {
      //check email & otp exists
      var user = await _userRepository.GetUserByEmailAsync(request.Email.ToLower());
      if (user == null)
      {
        return new ApiResponse<VerifyForgetPasswordResponseDto>(404, "Người dùng không tồn tại", null);
      }
      var otp = await _redisService.GetValueAsync($"otp:{request.Email.ToLower()}");
      if (request.Otp != otp)
      {
        return new ApiResponse<VerifyForgetPasswordResponseDto>(404, "Otp không đúng!", null);
      }
      string token = _jwtService.GenerateToken(user.Id, user.Email);
      return new ApiResponse<VerifyForgetPasswordResponseDto>(200, "Xác thực Otp quên mật khẩu thành công, vui lòng thay đổi mật khẩu mới!", new VerifyForgetPasswordResponseDto
      {
        Link =$"https://localhost:5082/User/ChangePassword?token={token}",
        Token = token
      });
    }

    public async Task<ApiResponse<string>> ChangePassword(ChangePasswordRequestDto request)
    {
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

    public async Task<ApiResponse<UserDto>> GetUser()
    {
      //check token
      var email = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(email))
      {
        return new ApiResponse<UserDto>(401, "Không thể xác thực người dùng!", null);
      }
      //get user by email
      var user = await GetUserByEmailAsync(email);
      var response = _mapper.Map<UserDto>(user);
      if (user == null)
      {
        return new ApiResponse<UserDto>(404, "Người dùng không tồn tại!", null);
      }
      return new ApiResponse<UserDto>(200, "Lấy thành công thông tin của bạn", response);
    }

    public async Task<ApiResponse<UserDto>> UpdateUser(UpdateUserRequestDto request)
    {
      //check image
      string? imageUrl = null;
      if (request.ImageUrl != null)
      {
        var uploadResult = await _imageService.UploadImage(new UploadImageRequest { Image = request.ImageUrl });
        if (uploadResult.Status != 201)
        {
          return new ApiResponse<UserDto>(400, "Tải ảnh lên thất bại!", null);
        }
        imageUrl = uploadResult.Data;
      }
      //check user
      var enail = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(enail))
      {
        return new ApiResponse<UserDto>(401, "Không thể xác thực người dùng!", null);
      }
      var user = await GetUserByEmailAsync(enail);
      if (user == null)
      {
        return new ApiResponse<UserDto>(404, "Người dùng không tồn tại!", null);
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
        return new ApiResponse<UserDto>(400, "Không có thông tin nào để cập nhật!", null);
      } else
      {
        await _userRepository.UpdateAsync(user);
      }
      var response = _mapper.Map<UserDto>(user);
      return new ApiResponse<UserDto>(200, "Cập nhật thành công thông tin của bạn!", response);
    }

    public async Task<List<UserReportResponseDto>> ReportOfUser()
    {
      //check user
      var enail = GetCurrentUserEmail();
      if (string.IsNullOrEmpty(enail))
      {
        return new List<UserReportResponseDto>();
      }
      var user = await GetUserByEmailAsync(enail);
      if (user == null)
      {
        return new List<UserReportResponseDto>();
      }
      var users = await _userRepository.GetUserById(user.Id);
      if (users == null)
      {
        return new List<UserReportResponseDto>();
      }
      //config Date and report
      var startDate = DateTime.UtcNow.AddDays(-7);
      var endDate = DateTime.UtcNow;
      var report = new List<UserReportResponseDto>();
      //export report
      int postCount = await _postRepository.CountPostsByUserId(users.Id, startDate, endDate);
      int commentCount = await _commentRepository.CountCommentsByUserId(users.Id, startDate, endDate);
      int likeCount = await _likeRepository.CountLikeByUserId(users.Id, startDate, endDate);
      int friendCount = await _friendRepository.CountFriendByUserId(users.Id, startDate, endDate);
      //response
      report.Add(new UserReportResponseDto 
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
    public byte[] ExportUserReportsToExcel(List<UserReportResponseDto> reports)
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
