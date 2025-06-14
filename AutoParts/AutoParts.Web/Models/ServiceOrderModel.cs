namespace AutoParts.Web.Models;

using AutoParts.Web.DTOs;
using AutoParts.Web.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ServiceOrderModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 1000 characters.")]
    public string Description { get; set; } = String.Empty;

    public ICollection<ServiceTaskModel> Tasks { get; set; } = new List<ServiceTaskModel>();

    [Required(ErrorMessage = "Order status is required.")]
    public OrderStatus Status { get; set; } = OrderStatus.New;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Order must be created for a given customer.")]
    public int CustomerId { get; set; }

    public CustomerShortDto Customer { get; set; } = null!;

    [Required(ErrorMessage = "Order must be create for specific vehicle.")]
    public int VehicleId { get; set; }

    public VehicleShortDto Vehicle { get; set; } = null!;

    public string? MechanicId { get; set; }

    public UserShortDto? Mechanic { get; set; }

    public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();

    [NotMapped]
    public decimal TotalCost => Tasks.Sum(task => task.TotalCost);
}
