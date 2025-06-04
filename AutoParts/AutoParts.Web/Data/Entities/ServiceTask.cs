namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ServiceTask
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = String.Empty;

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    [Range(0.01, 10000000.00)]
    public decimal LaborCost { get; set; }

    [Required]
    public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();

    [NotMapped]
    public decimal TotalCost => UsedParts.Sum(part => part.TotalPrice) + LaborCost;
}
