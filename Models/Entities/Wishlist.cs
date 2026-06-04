namespace E_project_DVD_Shop.Models.Entities;

public class Wishlist
{
    public int WishlistId { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
}
