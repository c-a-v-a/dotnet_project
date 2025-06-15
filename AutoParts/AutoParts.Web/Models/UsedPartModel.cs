namespace AutoParts.Web.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


public class UsedPartModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Used part must be a part of the task.")]
    public int ServiceTaskId { get; set; }

    [Required(ErrorMessage = "Part is required.")]
    public int PartId { get; set; }

    public PartModel? Part { get; set; } = null!;

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Quantity { get; set; }

    public decimal TotalPrice => (Part == null) ? 0 : Part.UnitPrice * Quantity;
}
