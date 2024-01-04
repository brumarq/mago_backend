namespace Application.ApplicationServices.Interfaces
{
    public interface IUserService
    {
        void CheckUserExistence(string userId);
        Task DeleteUser(string userId);
    }
}
