using System.ComponentModel.DataAnnotations;

namespace E_project_DVD_Shop.Models.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    [Required, StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(2000)]
    public string? AdminReply { get; set; }
}
