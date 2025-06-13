using AutoParts.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;


namespace AutoParts.Web.Models
{
    public class EditOrderStatusViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Status zlecenia")]
        [Required]
        public ServiceOrder.OrderStatus Status { get; set; }

        [Required]
        [Display(Name = "Nowy status")]
        public ServiceOrder.OrderStatus NewStatus { get; set; }
        public string? MechanicId { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsAssignedMechanic { get; set; }
    }
}
