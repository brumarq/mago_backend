namespace Application.ApplicationServices.Interfaces
{
    public interface IAuthenticationService
    {
        string? GetUserId();
        bool HasPermission(string permission);
        string GetToken();
        bool IsLoggedInUser();
    }
}