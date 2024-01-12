using Application.ApplicationServices.Authentization.Interfaces;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.ApplicationServices.Authentization
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
            var header = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(header))
                throw new BadRequestException("Invalid or missing Authorization header.");
            return header.Split(" ")[1];
        }

        public bool IsLoggedInUser()
        {
            return HasPermission("client") || HasPermission("admin");
        }
    }
}
