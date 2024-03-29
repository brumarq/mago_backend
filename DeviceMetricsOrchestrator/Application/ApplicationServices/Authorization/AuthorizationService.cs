﻿using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;

namespace Application.ApplicationServices.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUsersOnDevicesService _usersOnDevicesService;
        private readonly IAuthenticationService _authenticationService;
        public AuthorizationService(IAuthenticationService authenticationService, IUsersOnDevicesService usersOnDevicesService)
        {
            _authenticationService = authenticationService;
            _usersOnDevicesService = usersOnDevicesService;
        }

        public async Task<bool> IsDeviceAccessibleToUser(string loggedInUserId, int deviceId)
        {
            var isAdmin = _authenticationService.HasPermission("admin");

            var usersDevices = await _usersOnDevicesService.GetUsersOnDevicesByUserIdAsync(loggedInUserId!);

            return usersDevices.Any(uod => uod.Device?.Id == deviceId) || isAdmin;
        }

        public bool IsCorrectUserOrAdmin(string loggedInUserId, string userId)
        {
            var isAdmin = _authenticationService.HasPermission("admin");

            return loggedInUserId.Equals(userId) || isAdmin;
        }
    }
}
