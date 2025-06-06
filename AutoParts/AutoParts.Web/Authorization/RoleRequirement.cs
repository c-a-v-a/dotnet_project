namespace AutoParts.Web.Authorization;

using AutoParts.Web.Enums;
using Microsoft.AspNetCore.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public UserRole RequiredRole { get; }
    public RoleRequirement(UserRole role) => RequiredRole = role;
}
