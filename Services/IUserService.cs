using SocialMedia.Models.Dto;
using SocialMedia.Models.Dto.User;

namespace SocialMedia.Services
{
  public interface IUserService
  {
    Task<ApiResponse<string>> RegisterUser(RegisterUserRequestDto request);
    Task<ApiResponse<LoginUserResponseDto>> LoginUser(LoginUserRequestDto request);
    Task<ApiResponse<AuthUserResponseDto>> VerifyOtp(AuthUserRequestDto request);
    Task<ApiResponse<ForgetPasswordResponseDto>> ForgetPassword(ForgetPasswordRequestDto request);
    Task<ApiResponse<VerifyForgetPasswordResponseDto>> VerifyForgetPasword(VerifyForgetPasswordRequestDto request);
    Task<ApiResponse<string>> ChangePassword(ChangePasswordRequestDto request);
    Task<ApiResponse<UserDto>> GetUser();
    Task<ApiResponse<UserDto>> UpdateUser(UpdateUserRequestDto request);
    Task<List<UserReportResponseDto>> ReportOfUser();
    byte[] ExportUserReportsToExcel(List<UserReportResponseDto> reports);
  }
}
