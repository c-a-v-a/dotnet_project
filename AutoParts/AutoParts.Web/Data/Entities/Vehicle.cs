namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.Enums;

public class Vehicle
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = String.Empty;

    [Required]
    [MaxLength(50)]
    public string ModelName { get; set; } = String.Empty;

    [MaxLength(17)]
    public string? VIN { get; set; }

    [MaxLength(10)]
    public string? LicensePlate { get; set; }

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [MaxLength(30)]
    public string Color { get; set; } = String.Empty;

    public FuelType Fuel { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public Customer Customer { get; set; } = null!;
}
