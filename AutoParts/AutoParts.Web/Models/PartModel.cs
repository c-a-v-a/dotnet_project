namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;

public class PartModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Part name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Part name must be between 2 and 100 characters.")]
    public string Name { get; set; } = String.Empty;

    [Required(ErrorMessage = "Part type is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Part name must be between 2 and 100 characters.")]
    public string Type { get; set; } = String.Empty;

    [Required(ErrorMessage = "Unit price is required.")]
    [Range(0.01, 100000, ErrorMessage = "Unit price must be between 0.01 and 100000")]
    public decimal UnitPrice { get; set; }
}
