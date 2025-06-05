namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Vehicle
{
    public enum FuelType
    {
        Petrol,
        Diesel,
        Gas,
        Hybrid,
        Electric,
        Hydrogen,
        Other
    };

    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = String.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = String.Empty;

    [Required]
    [MaxLength(17)]
    public string? VIN { get; set; }

    [Required]
    [MaxLength(10)]
    public string? LicensePlate { get; set; }

    [Range(1886, 2100)]
    public int Year { get; set; }

    [MaxLength(20)]
    public string Color { get; set; } = String.Empty;

    public FuelType Fuel { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public Customer Customer { get; set; } = null!;
}
