using Microsoft.AspNetCore.Authorization;

namespace WebApp.Middleware.Authentication;

public abstract class HasPermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public string Issuer { get; }

    public HasPermissionRequirement(string permission, string issuer)
    {
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }
}