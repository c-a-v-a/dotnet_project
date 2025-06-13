using System.ComponentModel.DataAnnotations;

namespace AutoParts.Web.Models;

public class AddServiceTaskViewModel
{
    public int ServiceOrderId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 10000000.0)]
    public decimal LaborCost { get; set; }
}
