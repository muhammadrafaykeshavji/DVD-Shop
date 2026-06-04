namespace E_project_DVD_Shop.Data;

/// <summary>Official game-studio logos from <see cref="MediaCatalogLinks.StudioLogos"/>.</summary>
public static class GameStudioLogoUrls
{
    public static string? GetLogo(string name) => MediaCatalogLinks.GetStudioLogo(name);
}
