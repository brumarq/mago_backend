namespace Application.ApplicationServices.Interfaces
{
    public interface IUserService
    {
        Task CheckUserExistence(string userId);
        Task DeleteUser(string userId);
    }
}
