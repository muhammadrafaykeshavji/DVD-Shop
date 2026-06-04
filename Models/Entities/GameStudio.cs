namespace E_project_DVD_Shop.Models.Entities;

public class GameStudio
{
    public int GameStudioId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}
