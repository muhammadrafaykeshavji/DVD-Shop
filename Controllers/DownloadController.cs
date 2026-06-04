using E_project_DVD_Shop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Controllers;

[Authorize(Roles = "Member")]
public class DownloadController : Controller
{
    private readonly ApplicationDbContext _context;

    public DownloadController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var freeSongs = await _context.Songs
            .Include(s => s.Album)
            .Where(s => s.IsFree && s.DownloadUrl != null)
            .ToListAsync();

        var trailers = await _context.Songs
            .Where(s => s.PreviewUrl != null)
            .Select(s => new { s.Title, s.PreviewUrl, AlbumTitle = s.Album.Title })
            .Take(20)
            .ToListAsync();

        ViewBag.Trailers = trailers;
        return View(freeSongs);
    }
}
