namespace AutoParts.Web.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


public class AddUsedPartViewModel
{
    public int ServiceTaskId { get; set; }

    [Required]
    public int PartId { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; }

    public List<SelectListItem> AvailableParts { get; set; } = new();
}
