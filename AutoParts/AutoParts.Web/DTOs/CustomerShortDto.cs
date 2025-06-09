namespace AutoParts.Web.DTOs;

using System.ComponentModel.DataAnnotations;

public class CustomerShortDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
}
