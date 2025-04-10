using Microsoft.AspNetCore.Identity.Data;
using SocialMedia.Models.Dto.Response;

namespace SocialMedia.Services.Users
{
  public interface IUserService
  {
    Task<ApiResponse<String>> RegisterUser(RegisterRequest request);
  }
}
