using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using Models;

namespace GamingEventPublisherN
{
    public class GamingEventPublisher: IGamingEventPublisher
    {
        private readonly EventGridPublisherClient _eventGridClient;
        private readonly IConfiguration _configuration;

        public GamingEventPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
            var topicEndpoint = _configuration["EventGrid:TopicEndpoint"]!;
            var topicKey = _configuration["EventGrid:TopicKey"]!;

            _eventGridClient = new EventGridPublisherClient(
                new Uri(topicEndpoint),
                new AzureKeyCredential(topicKey));
        }

        public async Task PublishGamingEventAsync(GamingEvent gamingEvent)
        {
            try
            {
                var eventGridEvent = new EventGridEvent(
                    subject: $"gaming/events/{gamingEvent.EventType}",
                    eventType: $"Gaming.{gamingEvent.EventType}",
                    dataVersion: "1.0",
                    data: new BinaryData(gamingEvent)
                );

                await _eventGridClient.SendEventAsync(eventGridEvent);
                Console.WriteLine($"Published gaming event: {gamingEvent.EventId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish gaming event: {ex.Message}");
                throw;
            }
        }

        public Task PublishScheduledEventAsync(GamingEvent gamingEvent, DateTime publishTime)
        {
            // Implementation for scheduled events (could use Azure Scheduler or Timer Trigger)
            // This is a placeholder for scheduled event publishing
            return Task.CompletedTask;
        }
    }
}
