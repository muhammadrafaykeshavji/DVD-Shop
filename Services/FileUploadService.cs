namespace E_project_DVD_Shop.Services;

/// <summary>Saves uploaded images to wwwroot/uploads.</summary>
public class FileUploadService
{
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024;

    public FileUploadService(IWebHostEnvironment env) => _env = env;

    public async Task<string?> SaveImageAsync(IFormFile? file, string subFolder)
    {
        if (file == null || file.Length == 0) return null;
        if (file.Length > MaxFileSize) throw new InvalidOperationException("File exceeds 5MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Invalid file type. Allowed: jpg, png, gif, webp.");

        var folder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
        Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(folder, fileName);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/{subFolder}/{fileName}";
    }

    /// <summary>Downloads an external banner URL into wwwroot so product pages can load it reliably.</summary>
    public async Task<string?> SaveImageFromUrlAsync(string? url, string subFolder, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return url?.Trim();

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("CineVault/1.0");
            using var response = await client.GetAsync(url.Trim(), HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return url.Trim();

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
            var ext = contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".png"
            };
            var fromUrl = Path.GetExtension(new Uri(url.Trim()).AbsolutePath);
            if (AllowedExtensions.Contains(fromUrl.ToLowerInvariant()))
                ext = fromUrl.ToLowerInvariant();

            var folder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, fileName);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var file = new FileStream(path, FileMode.Create);
            await stream.CopyToAsync(file, cancellationToken);
            return $"/uploads/{subFolder}/{fileName}";
        }
        catch
        {
            return url.Trim();
        }
    }
}
