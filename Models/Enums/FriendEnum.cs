using System.Text.Json.Serialization;

namespace SocialMedia.Models.Enums
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum FriendEnum
  {
    Pending,
    Accepted,
    Declined
  }
}
