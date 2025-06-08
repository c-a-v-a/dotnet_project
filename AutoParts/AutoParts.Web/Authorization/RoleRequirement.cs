namespace AutoParts.Web.Authorization;

using AutoParts.Web.Enums;
using Microsoft.AspNetCore.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public IReadOnlyList<UserRole> RequiredRoles { get; }

    public RoleRequirement(params UserRole[] roles) => RequiredRoles = roles;
}
