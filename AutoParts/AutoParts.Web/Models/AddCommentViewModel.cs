using System.ComponentModel.DataAnnotations;

namespace AutoParts.Web.Models;

public class AddCommentViewModel
{
    public int ServiceOrderId { get; set; }

    [Required]
    [Display(Name = "Komentarz")]
    public string Text { get; set; }
}
