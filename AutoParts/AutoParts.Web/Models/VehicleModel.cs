namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.DTOs;
using AutoParts.Web.Enums;

public class VehicleModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The make is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The make name needs to be between 3 and 50 characters.")]
    public string Make { get; set; } = string.Empty;

    [Required(ErrorMessage = "The model is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The model name needs to be between 3 and 50 characters.")]
    public string ModelName { get; set; } = string.Empty;

    [Required(ErrorMessage = "The year of production is required.")]
    [Range(1900, 2100, ErrorMessage = "The year of production needs to be between 1900 and 2100.")]
    public int Year { get; set; }

    [MaxLength(30, ErrorMessage = "The model name needs to be up most 30 characters.")]
    public string? Color { get; set; } = string.Empty;

    [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "VIN must be exactly 17 characters (letters and numbers only, no I/O/Q).")]
    public string? VIN { get; set; }

    [MaxLength(10, ErrorMessage = "The license plate length needs to be up most 10 characters.")]
    public string? LicensePlate { get; set; }

    public string? ImageUrl { get; set; }

    public FuelType Fuel { get; set; }

    public int CustomerId { get; set; }

    public CustomerShortDto? Customer { get; set; } = null!;
}
