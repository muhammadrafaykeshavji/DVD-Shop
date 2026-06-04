namespace E_project_DVD_Shop.Models.Entities;

public class Song
{
    public int SongId { get; set; }
    public int AlbumId { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>Optional artwork; falls back to album cover when empty.</summary>
    public string? ImageUrl { get; set; }
    public int DurationSeconds { get; set; }
    public string? PreviewUrl { get; set; }
    public string? DownloadUrl { get; set; }
    public bool IsFree { get; set; }

    public Album Album { get; set; } = null!;
}
