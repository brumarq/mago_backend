using Application.ApplicationServices.Interfaces;

namespace Application.ApplicationServices
{
    public class AuthorizationsService : IAuthorizationsService
    {
        private readonly IUsersOnDevicesService _usersOnDevicesService;
        private readonly IAuthenticationService _authenticationService;
        
        public AuthorizationsService(IUsersOnDevicesService usersOnDevicesService, IAuthenticationService authenticationService)
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