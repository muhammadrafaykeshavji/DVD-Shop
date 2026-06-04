namespace E_project_DVD_Shop.Models.Entities;

public class Artist
{
    public int ArtistId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Album> Albums { get; set; } = new List<Album>();
}
