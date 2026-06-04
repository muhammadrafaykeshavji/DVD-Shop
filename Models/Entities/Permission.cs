namespace E_project_DVD_Shop.Models.Entities;

public class Permission
{
    public int PermissionId { get; set; }

    public string AdminId { get; set; } = string.Empty;
    public ApplicationUser Admin { get; set; } = null!;

    public string ModuleName { get; set; } = string.Empty;

    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}
