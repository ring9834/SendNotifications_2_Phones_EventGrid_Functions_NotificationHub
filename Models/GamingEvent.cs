using System.Text.Json.Serialization;

namespace Models
{
    public record GamingEvent
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("eventType")]
        public string? EventType { get; set; } // "TournamentStart", "NewGameRelease", "Maintenance", etc.

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("scheduledTime")]
        public DateTime ScheduledTime { get; set; }

        [JsonPropertyName("gameId")]
        public string? GameId { get; set; }

        [JsonPropertyName("priority")]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        [JsonPropertyName("targetAudience")]
        public TargetAudience TargetAudience { get; set; } = TargetAudience.AllUsers;
    }
}
