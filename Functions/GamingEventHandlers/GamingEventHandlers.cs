// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Models;
using NotificationService;
using Microsoft.Azure.WebJobs;

namespace GamingEventHandlers
{
    public class GamingEventHandlers
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<GamingEventHandlers> _logger;

        public GamingEventHandlers(INotificationService notificationService, ILogger<GamingEventHandlers> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [FunctionName("ProcessGamingEvent")]
        public async Task ProcessGamingEvent(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            try
            {
                var gamingEvent = eventGridEvent.Data.ToObjectFromJson<GamingEvent>();

                _logger.LogInformation($"Processing gaming event: {gamingEvent.EventId} - {gamingEvent.Title}");

                switch (gamingEvent.TargetAudience)
                {
                    case TargetAudience.AllUsers:
                        await _notificationService.SendNotificationToAllAsync(gamingEvent);
                        break;

                    case TargetAudience.PremiumUsers:
                        await SendToPremiumUsers(gamingEvent);
                        break;

                    case TargetAudience.SpecificGamePlayers:
                        await SendToGamePlayers(gamingEvent);
                        break;

                    case TargetAudience.InactiveUsers:
                        await SendToInactiveUsers(gamingEvent);
                        break;
                }

                _logger.LogInformation($"Successfully processed gaming event: {gamingEvent.EventId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing gaming event: {ex.Message}");
                throw;
            }
        }

        [FunctionName("HandleTournamentStart")]
        public async Task HandleTournamentStart(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            var gamingEvent = eventGridEvent.Data.ToObjectFromJson<GamingEvent>();

            if (gamingEvent.EventType == "TournamentStart")
            {
                _logger.LogInformation($"Sending tournament start notification: {gamingEvent.Title}");

                // Add tournament-specific logic
                gamingEvent.Priority = NotificationPriority.High;
                await _notificationService.SendNotificationToAllAsync(gamingEvent);
            }
        }

        [FunctionName("HandleNewGameRelease")]
        public async Task HandleNewGameRelease(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            var gamingEvent = eventGridEvent.Data.ToObjectFromJson<GamingEvent>();

            if (gamingEvent.EventType == "NewGameRelease")
            {
                _logger.LogInformation($"Sending new game release notification: {gamingEvent.Title}");

                // Target users interested in similar games
                await SendToGamePlayers(gamingEvent);
            }
        }

        private async Task SendToPremiumUsers(GamingEvent gamingEvent)
        {
            // In real implementation, query database for premium users
            _logger.LogInformation($"Sending to premium users: {gamingEvent.Title}");

            // For demo, send to all users but with premium tag
            var premiumMessage = gamingEvent with { Message = $"[PREMIUM] {gamingEvent.Message}" };
            await _notificationService.SendNotificationToPlatformAsync(DevicePlatform.Android, premiumMessage);
            await _notificationService.SendNotificationToPlatformAsync(DevicePlatform.iOS, premiumMessage);
        }

        private async Task SendToGamePlayers(GamingEvent gamingEvent)
        {
            _logger.LogInformation($"Sending to game players of {gamingEvent.GameId}: {gamingEvent.Title}");

            // Notification Hub will handle tagging by game ID
            var androidMessage = CreateTargetedAndroidNotification(gamingEvent);
            var appleMessage = CreateTargetedAppleNotification(gamingEvent);

            // This would use tag expressions like: $"game:{gamingEvent.GameId}"
            // Actual tag-based sending would be implemented in NotificationService
        }

        private async Task SendToInactiveUsers(GamingEvent gamingEvent)
        {
            _logger.LogInformation($"Sending to inactive users: {gamingEvent.Title}");

            // Special logic for re-engaging inactive users
            var reEngagementMessage = gamingEvent with
            {
                Message = $"We miss you! {gamingEvent.Message}",
                Priority = NotificationPriority.High
            };

            await _notificationService.SendNotificationToPlatformAsync(DevicePlatform.Android, reEngagementMessage);
            await _notificationService.SendNotificationToPlatformAsync(DevicePlatform.iOS, reEngagementMessage);
        }

        private object CreateTargetedAndroidNotification(GamingEvent gamingEvent)
        {
            // Implementation for targeted notifications
            return null;
        }

        private object CreateTargetedAppleNotification(GamingEvent gamingEvent)
        {
            // Implementation for targeted notifications
            return null;
        }
    }
}
