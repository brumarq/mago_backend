namespace Application.ApplicationServices.Interfaces;

public interface IApplicationStateService
{
    Task<bool> MicroservicesReady();
}