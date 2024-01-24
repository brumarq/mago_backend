using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.ApplicationServices
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly ILogger<NotificationHubService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly INotificationTokenService _notificationTokenService;
        private readonly string? _connectionString;
        private readonly string? _notificationHubPath;

        public NotificationHubService(ILogger<NotificationHubService> logger, IConfiguration configuration, IDeviceService deviceService, IUserService userService, INotificationTokenService notificationTokenService)
        {
            _logger = logger;
            _deviceService = deviceService;
            _userService = userService; 
            _notificationTokenService = notificationTokenService;
            _connectionString = configuration["NotificationHub:ConnectionString"];
            _notificationHubPath = configuration["NotificationHub:NotificationHubPath"];
        }
        public async Task SendNotificationToNotificationHub(NotificationResponseDTO notificationResponseDTO)
        {
            try
            {
                await _deviceService.CheckDeviceExistence(notificationResponseDTO.DeviceID);

                var usersOnDevice = await _deviceService.GetUsersOnDevicesByDeviceIdAsync(notificationResponseDTO.DeviceID);
                if(!usersOnDevice.Any())
                {
                    _logger.Log(LogLevel.Information, $"no users on device: {notificationResponseDTO.DeviceID}");
                    return;
                }

                var usersOnDeviceList = usersOnDevice.ToList();
                for (int i = usersOnDeviceList.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        await _userService.CheckUserExistence(usersOnDeviceList[i].UserId);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"{ex.Message} on userid {usersOnDevice.ElementAt(i).UserId}");
                        usersOnDeviceList.RemoveAt(i);
                    }
                }

                List<string> notificationTokens = new List<string>();
                foreach(var entry in usersOnDeviceList)
                {
                    var notificationTokenOnUser = await _notificationTokenService.GetNotificationTokensByUserIdAsync(entry.UserId);
                    notificationTokens.Add(notificationTokenOnUser.NotificationToken);
                }


                var hub = NotificationHubClient.CreateClientFromConnectionString(_connectionString, _notificationHubPath);
                var payload = $"{{ \"data\": {{ \"message\": \"{notificationResponseDTO.Message}\" }} }}";
                var notification = new FcmNotification(payload);

                foreach (string token in notificationTokens)
                {
                    await hub.SendDirectNotificationAsync(notification, token);
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
            }
        }
    }
}
