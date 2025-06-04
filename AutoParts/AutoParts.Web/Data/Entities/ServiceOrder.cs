namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ServiceOrder {
  public enum OrderStatus {
    New,
    InProgress,
    Finished,
    Cancelled
  };

  public int Id { get; set; }

  [Required]
  public string Description { get; set; } = String.Empty;

  public ICollection<ServiceTask> Tasks { get; set; } = new List<ServiceTask>();

  [Required]
  public OrderStatus Status { get; set; } = OrderStatus.New;

  public int? MechanicId { get; set; }

  public User? Mechanic { get; set; }

  public ICollection<Comment> Comments { get; set; } = new List<Comment>();

  [NotMapped]
  public decimal TotalCost => Tasks.Sum(task => task.TotalCost);
}
