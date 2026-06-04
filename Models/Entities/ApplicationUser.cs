using Microsoft.AspNetCore.Identity;

namespace E_project_DVD_Shop.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public decimal Balance { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
