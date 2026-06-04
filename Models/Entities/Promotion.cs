using System.ComponentModel.DataAnnotations;

namespace E_project_DVD_Shop.Models.Entities;

public class Promotion
{
    public int PromotionId { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}
