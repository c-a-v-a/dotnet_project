namespace AutoParts.Web.Data.Entities;

using AutoParts.Web.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ServiceOrder
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = String.Empty;

    public ICollection<ServiceTask> Tasks { get; set; } = new List<ServiceTask>();

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.New;

    public string? MechanicId { get; set; }

    public User? Mechanic { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = default!;

    [Required]
    public int VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = default!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [NotMapped]
    public decimal TotalCost => Tasks.Sum(task => task.TotalCost);
}
