using E_project_DVD_Shop.Models.Entities;

namespace E_project_DVD_Shop.Helpers;

public static class AlbumCreditHelper
{
    public static string GetCreditName(Album album) => ProductCatalogHelper.GetCreditName(album);
}
