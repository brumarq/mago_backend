using Microsoft.AspNetCore.Authorization;

namespace WebApp.Middleware.Authentication;

public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasPermissionRequirement requirement
    ) {
        // If user does not have the permissions claim, get out of here
        if (!context.User.HasClaim(c => c.Type == "permissions" && c.Issuer == requirement.Issuer))
            return Task.CompletedTask;

        // Get the permissions from the claim
        var permissions = context.User.FindAll(c => c.Type == "permissions" && c.Issuer == requirement.Issuer)
            .Select(c => c.Value);

        // Succeed if the permissions contain the required permission
        if (permissions.Any(p => p == requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}