namespace E_project_DVD_Shop.Models.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Album> Albums { get; set; } = new List<Album>();
    public ICollection<Game> Games { get; set; } = new List<Game>();
    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
