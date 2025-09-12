// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using NotificationService;

namespace GamingEventHandlers
{
    public class DeviceRegistrationHandlers
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<DeviceRegistrationHandlers> _logger;

        public DeviceRegistrationHandlers(INotificationService notificationService, ILogger<DeviceRegistrationHandlers> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }
        [FunctionName("RegisterUserDevice")]
        public async Task RegisterUserDevice(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            try
            {
                var deviceInfo = eventGridEvent.Data.ToObjectFromJson<UserDeviceInfo>();
                await _notificationService.RegisterUserDeviceAsync(deviceInfo);
                _logger.LogInformation($"Registered device for user: {deviceInfo.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user device");
                throw;
            }
        }

        [FunctionName("UnregisterUserDevice")]
        public async Task UnregisterUserDevice(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            try
            {
                var unregisterRequest = eventGridEvent.Data.ToObjectFromJson<UnregisterDeviceRequest>();
                await _notificationService.UnregisterUserDeviceAsync(unregisterRequest.UserId, unregisterRequest.DeviceToken);
                _logger.LogInformation($"Unregistered device for user: {unregisterRequest.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering user device");
                throw;
            }
        }
    }
}
