namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.Enums;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = String.Empty;

    [Required]
    [MaxLength(50)]
    public string SecondName { get; set; } = String.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Undefined;

    public ICollection<ServiceOrder>? AssignedOrders { get; set; }
}
