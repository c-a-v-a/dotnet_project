namespace AutoParts.Web.Models;

using System.ComponentModel.DataAnnotations;
using AutoParts.Web.DTOs;

public class CommentModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Text is required.")]
    [StringLength(1000, MinimumLength = 2, ErrorMessage = "Comment text mus be between 2 and 1000 characters.")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "Creation date is required")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Comment needs to have an author.")]
    public required string AuthorId { get; set; }

    public UserShortDto? Author { get; set; }

    [Required(ErrorMessage = "Comment needs to belong to an order.")]
    public required int ServiceOrderId { get; set; }
}
