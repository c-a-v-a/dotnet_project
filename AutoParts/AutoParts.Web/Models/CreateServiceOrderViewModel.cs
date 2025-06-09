using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CreateServiceOrderViewModel
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int VehicleId { get; set; }

    public string? MechanicId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public List<SelectListItem> Customers { get; set; } = new();
    public List<SelectListItem> Vehicles { get; set; } = new();
    public List<SelectListItem> Mechanics { get; set; } = new();
}
