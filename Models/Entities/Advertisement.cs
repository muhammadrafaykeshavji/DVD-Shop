using System.ComponentModel.DataAnnotations;
using E_project_DVD_Shop.Models;

namespace E_project_DVD_Shop.Models.Entities;

public class Advertisement
{
    public int AdId { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [StringLength(500)]
    public string? LinkUrl { get; set; }

    public AdPosition Position { get; set; }

    public bool IsActive { get; set; } = true;
}
