using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAuthorizationService = Application.ApplicationServices.Interfaces.IAuthorizationService;

namespace Application.ApplicationServices
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IDeviceService _deviceService;
        private readonly IAuthenticationService _authenticationService;

        public AuthorizationService(IAuthenticationService authenticationService, IDeviceService deviceService)
        {
            _authenticationService = authenticationService;
            _deviceService = deviceService;
        }

        public async Task<bool> IsNotificationAccessibleToUser(string loggedInUserId, NotificationResponseDTO notificationResponseDTO)
        {
            var isAdmin = _authenticationService.HasPermission("admin");
            if (isAdmin)
                return true;

            int deviceId = notificationResponseDTO.DeviceID;
            var userOnDeviceEntries = await _deviceService.GetUserOnDeviceEntryByUserId(loggedInUserId);

            return userOnDeviceEntries.Any(uod => uod.DeviceId == notificationResponseDTO.DeviceID);
        }
    }
}
