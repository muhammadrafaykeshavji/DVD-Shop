using E_project_DVD_Shop.Models.Entities;

namespace E_project_DVD_Shop.Helpers;

public static class NewsQueryExtensions
{
    public static IQueryable<News> PublicArticles(this IQueryable<News> query) =>
        query.Where(n => n.IsActive
            && !n.Title.StartsWith("__")
            && (n.Category == NewsCategories.Gaming
                || n.Category == NewsCategories.Music
                || n.Category == NewsCategories.Film));
}
