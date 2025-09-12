using System.Text.Json.Serialization;

namespace Models
{
    public class UserDeviceInfo
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("deviceToken")]
        public string? DeviceToken { get; set; }

        [JsonPropertyName("platform")]
        public DevicePlatform Platform { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("preferredLanguage")]
        public string PreferredLanguage { get; set; } = "en";

        [JsonPropertyName("gamePreferences")]
        public string[] GamePreferences { get; set; } = Array.Empty<string>();
    }
}
