namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.Enums;

public class UserModel
{
    public UserModel() { }

    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Firs name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Firs name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(50, ErrorMessage = "Email can't exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role")]
    public UserRole Role { get; set; }
}
