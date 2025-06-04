namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UsedPart {
  public int Id { get; set; }

  [Required]
  public int PartId { get; set; }

  [Required]
  public Part Part { get; set; } = null!;

  [Required]
  public int Quantity { get; set; }

  [Required]
  public int ServiceTaskId { get; set; }

  [Required]
  public ServiceTask ServiceTask { get; set; } = null!;

  [NotMapped]
  public decimal TotalPrice => Part.UnitPrice * Quantity;
}
