using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_project_DVD_Shop.Models.Entities;

public class Game
{
    public int GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int GameStudioId { get; set; }
    public int CategoryId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
  /// <summary>Optional artwork; falls back to cover when empty.</summary>
    public string? ImageUrl { get; set; }
    public string? PreviewUrl { get; set; }
    public string? Description { get; set; }

    [ValidateNever] public GameStudio GameStudio { get; set; } = null!;
    [ValidateNever] public Category Category { get; set; } = null!;
    [ValidateNever] public ICollection<Product> Products { get; set; } = new List<Product>();
}
