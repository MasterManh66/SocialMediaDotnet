using System.Text.Json.Serialization;

namespace SocialMedia.Models.Enums
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum PostEnum
  {
    Public,
    Private,
    Friends
  }
}
