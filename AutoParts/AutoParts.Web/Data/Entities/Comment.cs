using System.ComponentModel.DataAnnotations;

namespace AutoParts.Web.Data.Entities;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public required string AuthorId { get; set; }

    public User Author { get; set; } = null!;

    [Required]
    public required int ServiceOrderId { get; set; }

    public ServiceOrder ServiceOrder { get; set; } = null!;
}
