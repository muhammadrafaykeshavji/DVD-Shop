using System.ComponentModel.DataAnnotations;
using E_project_DVD_Shop.Models;

namespace E_project_DVD_Shop.Models.Entities;

public class ForumPost
{
    [Key]
    public int PostId { get; set; }

    public ForumChannel Channel { get; set; } = ForumChannel.General;

    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int? ParentPostId { get; set; }
    public ForumPost? ParentPost { get; set; }
    public ICollection<ForumPost> Replies { get; set; } = new List<ForumPost>();
}
