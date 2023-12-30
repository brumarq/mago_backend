using Application.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.ApplicationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public bool HasPermission(string permission)
        {
            return _httpContextAccessor.HttpContext.User.HasClaim(c => c.Type == "permissions" && c.Value == permission);
        }

        public string GetToken()
        {
            return _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
        }
    }
}
