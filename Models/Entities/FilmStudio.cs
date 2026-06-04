namespace E_project_DVD_Shop.Models.Entities;

public class FilmStudio
{
    public int FilmStudioId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
