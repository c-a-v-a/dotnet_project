namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ServiceTask
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = String.Empty;

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    [Range(0.01, 10000000.00)]
    public decimal LaborCost { get; set; }

    public ICollection<UsedPart> UsedParts { get; set; } = new List<UsedPart>();

    [NotMapped]
    public decimal TotalCost => UsedParts.Sum(part => part.TotalPrice) + LaborCost;

    [Required]
    public int ServiceOrderId { get; set; }

    public ServiceOrder ServiceOrder { get; set; } = null!;
}
