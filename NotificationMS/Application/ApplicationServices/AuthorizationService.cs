using Application.ApplicationServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthorizationService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public bool IsCorrectUserOrAdmin(string loggedInUserId, string userId)
        {
            var isAdmin = _authenticationService.HasPermission("admin");

            return loggedInUserId.Equals(userId) || isAdmin;
        }
    }
}
