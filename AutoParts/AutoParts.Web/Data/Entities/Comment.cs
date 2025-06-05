namespace AutoParts.Web.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public string AuthorId { get; set; } = String.Empty;

    [Required]
    public User Author { get; set; } = null!;

    [Required]
    public string Content { get; set; } = String.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public int ServiceOrderId { get; set; }

    [Required]
    public ServiceOrder ServiceOrder { get; set; } = null!;
}
