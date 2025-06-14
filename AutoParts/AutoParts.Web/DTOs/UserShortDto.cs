namespace AutoParts.Web.DTOs;

using System.ComponentModel.DataAnnotations;

public class UserShortDto
{
    public required string Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;
}
