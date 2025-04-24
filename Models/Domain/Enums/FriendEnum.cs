using System.Text.Json.Serialization;

namespace SocialMedia.Models.Domain.Enums
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum FriendEnum
  {
    Pending,
    Accepted,
    Declined
  }
}
