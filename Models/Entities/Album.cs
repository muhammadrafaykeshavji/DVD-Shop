using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_project_DVD_Shop.Models.Entities;

/// <summary>Music albums only. Games and movies live in <see cref="Game"/> / <see cref="Movie"/>.</summary>
public class Album
{
    public int AlbumId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ArtistId { get; set; }
    public int? MusicStudioId { get; set; }
    public int CategoryId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
    /// <summary>Wide landscape promo art for blurred product page background.</summary>
    public string? BannerImageUrl { get; set; }
    /// <summary>How visible the blurred banner is on the product page (0–100).</summary>
    public int BannerVisibility { get; set; } = 50;
    public string? Description { get; set; }

    [ValidateNever] public Artist? Artist { get; set; }
    [ValidateNever] public MusicStudio? MusicStudio { get; set; }
    [ValidateNever] public Category Category { get; set; } = null!;
    [ValidateNever] public ICollection<Product> Products { get; set; } = new List<Product>();
    [ValidateNever] public ICollection<Song> Songs { get; set; } = new List<Song>();
}
