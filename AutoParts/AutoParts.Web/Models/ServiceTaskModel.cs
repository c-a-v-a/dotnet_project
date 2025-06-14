namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ServiceTaskModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Task name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Task name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Task description is required.")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Task description must be between 2 and 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Labor cost is required.")]
    [Range(0.01, 10000000.0, ErrorMessage = "Task labor cost must be between 0.01 and 10000000.0.")]
    public decimal LaborCost { get; set; }

    public ICollection<UsedPartModel> UsedParts { get; set; } = new List<UsedPartModel>();

    [Required(ErrorMessage = "Task must be a part of service order.")]
    public int ServiceOrderId { get; set; }

    public decimal TotalCost => UsedParts.Sum(part => part.TotalPrice) + LaborCost;
}
