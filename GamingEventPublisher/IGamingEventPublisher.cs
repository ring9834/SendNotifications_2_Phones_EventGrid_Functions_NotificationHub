using Models;

namespace GamingEventPublisherN
{
    public interface IGamingEventPublisher
    {
        Task PublishGamingEventAsync(GamingEvent gamingEvent);
        Task PublishScheduledEventAsync(GamingEvent gamingEvent, DateTime publishTime);
    }
}
