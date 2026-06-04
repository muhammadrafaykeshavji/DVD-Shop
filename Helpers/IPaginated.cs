namespace E_project_DVD_Shop.Helpers;

public interface IPaginated
{
    int PageIndex { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}
