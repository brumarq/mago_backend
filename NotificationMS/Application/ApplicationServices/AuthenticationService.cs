using Application.ApplicationServices.Interfaces;
using Application.Exceptions;
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
            var headers = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(headers))
                throw new BadRequestException("Invalid or missing Authorization header.");
            return headers.Split(" ")[1];
        }

        public bool IsLoggedInUser()
        {
            return HasPermission("client") || HasPermission("admin");
        }
    }
}
