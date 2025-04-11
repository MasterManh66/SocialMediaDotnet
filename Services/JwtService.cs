using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace SocialMedia.Services
{
  public class JwtService : IJwtService
  {
    private readonly IConfiguration _configuration;
    public JwtService(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    public string GenerateToken(int userId, string email)
    {
      var jwtSettings = _configuration.GetSection("Jwt");
      var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("JWT Key is missing")));
      var credentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
        signingCredentials: credentials
      );
      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
      var jwtSettings = _configuration.GetSection("Jwt");
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("JWT Key is missing"));

      try
      {
        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        return principal;
      }
      catch
      {
        return null;
      }
    }

    public string? GetUserIdFromToken(string token)
    {
      var principal = ValidateToken(token);
      return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string? GetEmailFromToken(string token)
    {
      var principal = ValidateToken(token);
      return principal?.FindFirst(ClaimTypes.Email)?.Value;
    }
  }
}
