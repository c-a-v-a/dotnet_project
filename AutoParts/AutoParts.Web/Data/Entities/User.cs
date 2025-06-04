namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class User {
  public enum UserRole {
    Admin,
    Receptionist,
    Mechanic
  };

  public int Id { get; set; }

  [Required]
  [MaxLength(50)]
  public string Name { get; set; } = String.Empty;

  [Required]
  [MaxLength(50)]
  public string SecondName { get; set; } = String.Empty;

  [Required]
  [Phone]
  [MaxLength(20)]
  public string PhoneNumber { get; set; } = String.Empty;

  [Required]
  [EmailAddress]
  [MaxLength(50)]
  public string Email { get; set; } = String.Empty;

  [Required]
  public UserRole Role { get; set; }

  public ICollection<ServiceOrder>? AssignedOrders { get; set; }
}
