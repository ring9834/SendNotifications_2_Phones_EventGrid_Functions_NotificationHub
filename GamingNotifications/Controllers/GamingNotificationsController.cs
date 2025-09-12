using GamingEventPublisherN;
using Microsoft.AspNetCore.Mvc;
using Models;
using NotificationService;
using System;
using System.Threading.Tasks;

namespace GamingNotifications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamingNotificationsController : ControllerBase
    {
        private readonly IGamingEventPublisher _eventPublisher;
        private readonly INotificationService _notificationService;

        public GamingNotificationsController(IGamingEventPublisher eventPublisher, INotificationService notificationService)
        {
            _eventPublisher = eventPublisher;
            _notificationService = notificationService;
        }

        [HttpPost("events")]
        public async Task<IActionResult> CreateGamingEvent([FromBody] GamingEvent gamingEvent)
        {
            try
            {
                if (string.IsNullOrEmpty(gamingEvent.EventId))
                    gamingEvent.EventId = Guid.NewGuid().ToString();

                await _eventPublisher.PublishGamingEventAsync(gamingEvent);

                return Ok(new
                {
                    EventId = gamingEvent.EventId,
                    Message = "Gaming event published successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("devices/register")]
        public async Task<IActionResult> RegisterDevice([FromBody] UserDeviceInfo deviceInfo)
        {
            try
            {
                await _notificationService.RegisterUserDeviceAsync(deviceInfo);
                return Ok(new { Message = "Device registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("test/notification")]
        public async Task<IActionResult> SendTestNotification()
        {
            try
            {
                var testEvent = new GamingEvent
                {
                    EventType = "TestNotification",
                    Title = "Test Gaming Event",
                    Message = "This is a test notification from the gaming platform",
                    ScheduledTime = DateTime.UtcNow.AddHours(1),
                    GameId = "test-game-001",
                    Priority = NotificationPriority.Normal,
                    TargetAudience = TargetAudience.AllUsers
                };

                await _notificationService.SendNotificationToAllAsync(testEvent);

                return Ok(new { Message = "Test notification sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
