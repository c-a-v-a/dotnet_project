namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = String.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = String.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = String.Empty;

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
