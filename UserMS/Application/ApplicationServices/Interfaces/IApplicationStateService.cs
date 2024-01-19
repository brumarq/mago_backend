namespace Application.ApplicationServices.Interfaces;

public interface IApplicationStateService
{
    Task<bool> Auth0Available();
}