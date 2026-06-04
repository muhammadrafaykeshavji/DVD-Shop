using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_project_DVD_Shop.Models.Entities;

public class Product
{
    public int ProductId { get; set; }
    public int? AlbumId { get; set; }
    public int? GameId { get; set; }
    public int? MovieId { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int? ProducerId { get; set; }
    public int? SupplierId { get; set; }

    [ValidateNever] public Album? Album { get; set; }
    [ValidateNever] public Game? Game { get; set; }
    [ValidateNever] public Movie? Movie { get; set; }
    [ValidateNever] public Producer? Producer { get; set; }
    [ValidateNever] public Supplier? Supplier { get; set; }
    [ValidateNever] public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    [ValidateNever] public ICollection<Review> Reviews { get; set; } = new List<Review>();
    [ValidateNever] public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
