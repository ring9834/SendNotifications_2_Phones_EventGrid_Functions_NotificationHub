using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;

namespace NotificationService
{
    public class NotificationHubService : INotificationService
    {
        private readonly NotificationHubClient _notificationHubClient;
        private readonly ILogger<NotificationHubService> _logger;
        private readonly string _notificationHubName;
        private readonly string _notificationHubConnectionString;

        public NotificationHubService(IConfiguration configuration, ILogger<NotificationHubService> logger)
        {
            _logger = logger;
            _notificationHubConnectionString = configuration["NotificationHub:ConnectionString"]!;
            _notificationHubName = configuration["NotificationHub:Name"]!;

            _notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(
                _notificationHubConnectionString,
                _notificationHubName);
        }

        public async Task SendNotificationToAllAsync(GamingEvent gamingEvent)
        {
            try
            {
                var androidMessage = CreateAndroidNotification(gamingEvent);
                var appleMessage = CreateAppleNotification(gamingEvent);

                // Send to both platforms
                var androidResult = await _notificationHubClient.SendNotificationAsync(androidMessage);
                var appleResult = await _notificationHubClient.SendNotificationAsync(appleMessage);

                _logger.LogInformation($"Sent notification to all users. Android: {androidResult.State}, iOS: {appleResult.State}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to all users");
                throw;
            }
        }

        public async Task SendNotificationToUserAsync(string userId, GamingEvent gamingEvent)
        {
            try
            {
                var tag = $"userid:{userId}";
                var androidMessage = CreateAndroidNotification(gamingEvent);
                var appleMessage = CreateAppleNotification(gamingEvent);

                // Send to user's specific tag
                var androidResult = await _notificationHubClient.SendNotificationAsync(androidMessage, tag);
                var appleResult = await _notificationHubClient.SendNotificationAsync(appleMessage, tag);

                _logger.LogInformation($"Sent notification to user {userId}. Android: {androidResult.State}, iOS: {appleResult.State}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
                throw;
            }
        }

        public async Task SendNotificationToPlatformAsync(DevicePlatform platform, GamingEvent gamingEvent)
        {
            try
            {
                switch (platform)
                {
                    case DevicePlatform.Android:
                        var androidMessage = CreateAndroidNotification(gamingEvent);
                        await _notificationHubClient.SendNotificationAsync(androidMessage);
                        break;

                    case DevicePlatform.iOS:
                        var appleMessage = CreateAppleNotification(gamingEvent);
                        await _notificationHubClient.SendNotificationAsync(appleMessage);
                        break;

                    default:
                        throw new NotSupportedException($"Platform {platform} not supported");
                }

                _logger.LogInformation($"Sent notification to {platform} platform");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to {platform} platform");
                throw;
            }
        }

        public async Task RegisterUserDeviceAsync(UserDeviceInfo deviceInfo)
        {
            try
            {
                Installation installation = new Installation
                {
                    InstallationId = deviceInfo.DeviceToken,
                    Platform = deviceInfo.Platform switch
                    {
                        DevicePlatform.Android => NotificationPlatform.Fcm,
                        DevicePlatform.iOS => NotificationPlatform.Apns,
                        _ => throw new NotSupportedException($"Platform {deviceInfo.Platform} not supported")
                    },
                    PushChannel = deviceInfo.DeviceToken,
                    Tags = new List<string>
                    {
                        $"userid:{deviceInfo.UserId}",
                        $"platform:{deviceInfo.Platform.ToString().ToLower()}",
                        $"language:{deviceInfo.PreferredLanguage}"
                    }
                };

                // Add game preferences as tags
                foreach (var gameId in deviceInfo.GamePreferences)
                {
                    installation.Tags.Add($"game:{gameId}");
                }

                // creates or updates a device installation in Azure Notification Hubs,
                // which serves as the central registry for all devices that should receive notifications
                // Without this call:
                // - Device won't be registered
                // - Notifications won't reach the device
                // - No targeting or segmentation possible
                await _notificationHubClient.CreateOrUpdateInstallationAsync(installation);
                _logger.LogInformation($"Registered device for user {deviceInfo.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering device for user {deviceInfo.UserId}");
                throw;
            }
        }

        public async Task UnregisterUserDeviceAsync(string userId, string deviceToken)
        {
            try
            {
                await _notificationHubClient.DeleteInstallationAsync(deviceToken);
                _logger.LogInformation($"Unregistered device for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unregistering device for user {userId}");
                throw;
            }
        }

        private AppleNotification CreateAppleNotification(GamingEvent gamingEvent)
        {
            var payload = new
            {
                aps = new
                {
                    alert = new
                    {
                        title = gamingEvent.Title,
                        body = gamingEvent.Message
                    },
                    sound = "default",
                    badge = 1,
                    category = gamingEvent.EventType,
                    contentAvailable = 1
                },
                eventId = gamingEvent.EventId,
                gameId = gamingEvent.GameId,
                scheduledTime = gamingEvent.ScheduledTime.ToString("o")
            };

            return new AppleNotification(Newtonsoft.Json.JsonConvert.SerializeObject(payload));
        }

        private FcmNotification CreateAndroidNotification(GamingEvent gamingEvent)
        {
            var payload = new
            {
                data = new
                {
                    title = gamingEvent.Title,
                    message = gamingEvent.Message,
                    eventId = gamingEvent.EventId,
                    gameId = gamingEvent.GameId,
                    scheduledTime = gamingEvent.ScheduledTime.ToString("o"),
                    eventType = gamingEvent.EventType,
                    priority = gamingEvent.Priority.ToString()
                },
                notification = new
                {
                    title = gamingEvent.Title,
                    body = gamingEvent.Message,
                    sound = "default",
                    click_action = "OPEN_GAMING_EVENT"
                }
            };

            return new FcmNotification(Newtonsoft.Json.JsonConvert.SerializeObject(payload));
        }
    }
}
