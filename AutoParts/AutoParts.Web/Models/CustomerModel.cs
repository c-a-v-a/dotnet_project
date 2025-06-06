namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.Data.Entities;

public class CustomerModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane")]
    [MaxLength(50)]
    [Display(Name = "Imię")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    [Display(Name = "Drugie imię")]
    public string SecondName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane")]
    [MaxLength(50)]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon jest wymagany")]
    [Phone(ErrorMessage = "Nieprawidłowy format telefonu")]
    [Display(Name = "Telefon")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email jest wymagany")]
    [EmailAddress(ErrorMessage ="Niepoprawny format e-mail")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = String.Empty;

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
