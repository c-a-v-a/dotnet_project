namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.Enums;

public class UserModel
{
    public UserModel() { }

    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
