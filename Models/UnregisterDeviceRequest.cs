using System.Text.Json.Serialization;

namespace Models
{
    public class UnregisterDeviceRequest
    {
        public string? UserId { get; set; }
        public string? DeviceToken { get; set; }
    }
}
