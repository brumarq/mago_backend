namespace Application.ApplicationServices.Interfaces
{
    public interface IUserService
    {
        Task<HttpResponseMessage> GetUserExistenceStatus(string userId);
    }
}
