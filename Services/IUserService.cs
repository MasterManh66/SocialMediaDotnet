using SocialMedia.Models.Dto.Request;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services
{
  public interface IUserService
  {
    Task<ApiResponse<string>> RegisterUser(RegisterUserRequest request);
    Task<ApiResponse<LoginResponse>> LoginUser(LoginUserRequest request);
    Task<ApiResponse<AuthResponse>> VerifyOtp(AuthRequest request);
    Task<ApiResponse<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordRequest request);
    Task<ApiResponse<VerifyForgetPasswordResponse>> VerifyForgetPasword(VerifyForgetPasswordRequest request);
    Task<ApiResponse<string>> ChangePassword(ChangePasswordRequest request);
    Task<ApiResponse<UserResponse>> GetUser();
    Task<ApiResponse<UserResponse>> UpdateUser(UserRequest request);
    Task<List<UserReportResponse>> ReportOfUser();
    byte[] ExportUserReportsToExcel(List<UserReportResponse> reports);
  }
}
