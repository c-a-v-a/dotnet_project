namespace AutoParts.Web.Authorization;

using System.Threading.Tasks;
using AutoParts.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

public class RoleHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly UserManager<User> _userManager;

    public RoleHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);

        if (user != null && requirement.RequiredRoles.Contains(user.Role))
        {
            context.Succeed(requirement);
        }
    }
}
