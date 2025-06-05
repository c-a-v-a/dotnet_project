namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;

public class VehicleModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Marka pojazdu jest wymagana")]
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model pojazdu jest wymagany")]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2030)]
    public int Year { get; set; }

    [MaxLength(20)]
    public string Color { get; set; } = string.Empty;

    [MaxLength(17)]
    public string? VIN { get; set; }

    [MaxLength(10)]
    public string? LicensePlate { get; set; }

    [Required]
    public int CustomerId { get; set; }
}
