using StackExchange.Redis;

namespace SocialMedia.Services
{
  public class RedisService : IRedisService
  {
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer redis)
    {
      _database = redis.GetDatabase();
    }
    public async Task SetValueAsync(string key, string otp, TimeSpan expiry)
    {
      await _database.StringSetAsync(key, otp, expiry);
    }

    public async Task<string?> GetValueAsync(string key)
    {
      return await _database.StringGetAsync(key);
    }

    public async Task<bool> DeleteOtpAsync(string key)
    {
      return await _database.KeyDeleteAsync(key);
    }
  }
}
