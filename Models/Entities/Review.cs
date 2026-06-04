using System.ComponentModel.DataAnnotations;

namespace E_project_DVD_Shop.Models.Entities;

public class Review
{
    public int ReviewId { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
