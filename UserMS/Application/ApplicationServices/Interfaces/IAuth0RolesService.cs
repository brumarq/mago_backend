using Application.DTOs;
namespace Application.ApplicationServices.Interfaces;

public interface IAuth0RolesService
{
    Task<string> GetRole(string userId);
    Task AssignRole(string roleName, string userId);
    Task UnassignRoleAsync(string roleName, string userId);
}