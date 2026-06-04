using E_project_DVD_Shop.Models;

namespace E_project_DVD_Shop.Models.Entities;

public class News
{
    public int NewsId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public NewsType Type { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    /// <summary>Original article URL when synced from RSS.</summary>
    public string? SourceUrl { get; set; }

    /// <summary>Feed source label, e.g. IGN or Deadline.</summary>
    public string? SourceName { get; set; }

    public string? ImageUrl { get; set; }

    /// <summary>Stable id for RSS upsert, e.g. rss:ign:...</summary>
    public string? ExternalId { get; set; }
}
