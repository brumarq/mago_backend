namespace Application.ApplicationServices.Interfaces
{
    public interface IAuthorizationsService
    {
        Task<bool> IsDeviceAccessibleToUser(string loggedInUserId, int deviceId);
        bool IsCorrectUserOrAdmin(string loggedInUserId, string userId);
    }
}