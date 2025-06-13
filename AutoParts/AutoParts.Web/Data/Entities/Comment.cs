using System.ComponentModel.DataAnnotations;

namespace AutoParts.Web.Data.Entities;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public string AuthorId { get; set; }

    public User Author { get; set; }

    public int ServiceOrderId { get; set; }

    public ServiceOrder ServiceOrder { get; set; }
}
