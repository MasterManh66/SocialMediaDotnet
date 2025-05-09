namespace SocialMedia.Services
{
  public interface IRedisService
  {
    Task SetValueAsync(string key, string otp, TimeSpan expiry);
    Task<string?> GetValueAsync(string key);
    Task<bool> DeleteOtpAsync(string key);
  }
}
