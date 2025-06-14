﻿namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoParts.Web.DTOs;
using AutoParts.Web.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ServiceOrderModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 1000 characters.")]
    public string Description { get; set; } = String.Empty;

    public List<ServiceTaskModel> Tasks { get; set; } = new List<ServiceTaskModel>();

    [Required(ErrorMessage = "Order status is required.")]
    public OrderStatus Status { get; set; } = OrderStatus.New;

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
    public DateTime? StartDate { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
    public DateTime? EndDate { get; set; }

    public int CustomerId { get; set; } = 0;

    public CustomerShortDto? Customer { get; set; } = null;

    [Required(ErrorMessage = "Order must be create for specific vehicle.")]
    public int VehicleId { get; set; }

    public VehicleShortDto? Vehicle { get; set; } = null;

    public string? MechanicId { get; set; }

    public UserShortDto? Mechanic { get; set; }

    public List<CommentModel> Comments { get; set; } = new List<CommentModel>();

    [NotMapped]
    public decimal TotalCost => Tasks.Sum(task => task.TotalCost);
}
