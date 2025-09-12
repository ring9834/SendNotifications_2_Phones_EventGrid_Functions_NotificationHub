using Models;

namespace NotificationService
{
    public interface INotificationService
    {
        Task SendNotificationToAllAsync(GamingEvent gamingEvent);
        Task SendNotificationToUserAsync(string userId, GamingEvent gamingEvent);
        Task SendNotificationToPlatformAsync(DevicePlatform platform, GamingEvent gamingEvent);
        Task RegisterUserDeviceAsync(UserDeviceInfo deviceInfo);
        Task UnregisterUserDeviceAsync(string userId, string deviceToken);
    }
}
