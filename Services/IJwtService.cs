using System.Security.Claims;

namespace SocialMedia.Services
{
  public interface IJwtService
  {
    string GenerateToken(int userId, string email);
    ClaimsPrincipal? ValidateToken(string token);
    string? GetUserIdFromToken(string token);
    string? GetEmailFromToken(string token);
  }
}
