namespace AutoParts.Web.DTOs;

using System.ComponentModel.DataAnnotations;

public class VehicleShortDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The make is required.")]
    [StringLength(3, MinimumLength = 50, ErrorMessage = "The make name needs to be between 3 and 50 characters.")]
    public string Make { get; set; } = string.Empty;

    [Required(ErrorMessage = "The model is required.")]
    [StringLength(3, MinimumLength = 50, ErrorMessage = "The model name needs to be between 3 and 50 characters.")]
    public string ModelName { get; set; } = string.Empty;

    [Required(ErrorMessage = "The year of production is required.")]
    [Range(1900, 2100, ErrorMessage = "The year of production needs to be between 1900 and 2100.")]
    public int Year { get; set; }
}
