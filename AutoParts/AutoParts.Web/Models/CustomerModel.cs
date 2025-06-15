namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.DTOs;

public class CustomerModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Incorrect phone number format.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Incorrect email address format.")]
    public string Email { get; set; } = String.Empty;

    public ICollection<VehicleShortDto> Vehicles { get; set; } = new List<VehicleShortDto>();
}
