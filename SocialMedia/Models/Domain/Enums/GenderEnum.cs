using System.Text.Json.Serialization;

namespace SocialMedia.Models.Domain.Enums
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum GenderEnum
    {
        Male,
        FeMale,
        Other 
    }
}
