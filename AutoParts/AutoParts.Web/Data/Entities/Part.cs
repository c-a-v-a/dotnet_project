namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Part {
  public int Id { get; set; }

  [Required]
  public string Name { get; set; } = String.Empty;

  [Required]
  [Column(TypeName = "decimal(8,2)")]
  [Range(0.01, 100000.00)]
  public decimal UnitPrice { get; set; }
}
