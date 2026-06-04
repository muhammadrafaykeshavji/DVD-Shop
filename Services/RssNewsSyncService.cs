using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using E_project_DVD_Shop.Data;
using E_project_DVD_Shop.Models;
using E_project_DVD_Shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Services;

public sealed class RssFeedSource
{
    public required string Key { get; init; }
    public required string FeedUrl { get; init; }
    public required string Category { get; init; }
    public required string SourceName { get; init; }
    public NewsType Type { get; init; } = NewsType.International;
}

public class RssNewsSyncService
{
    public static readonly RssFeedSource[] DefaultFeeds =
    [
        new()
        {
            Key = "ign",
            FeedUrl = "https://feeds.ign.com/ign/games-all",
            Category = "Games",
            SourceName = "IGN",
            Type = NewsType.International
        },
        new()
        {
            Key = "pitchfork",
            FeedUrl = "https://pitchfork.com/feed/feed-news/rss",
            Category = "Music",
            SourceName = "Pitchfork",
            Type = NewsType.International
        },
        new()
        {
            Key = "deadline",
            FeedUrl = "https://deadline.com/feed",
            Category = "Movies",
            SourceName = "Deadline",
            Type = NewsType.International
        }
    ];

    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RssNewsSyncService> _logger;

    public RssNewsSyncService(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<RssNewsSyncService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> SyncAllAsync(CancellationToken cancellationToken = default)
    {
        if (!_configuration.GetValue("RssNews:Enabled", true))
            return 0;

        var maxPerFeed = _configuration.GetValue("RssNews:MaxItemsPerFeed", 12);
        var total = 0;

        foreach (var feed in DefaultFeeds)
        {
            try
            {
                total += await SyncFeedAsync(feed, maxPerFeed, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RSS sync failed for {Feed} ({Url})", feed.SourceName, feed.FeedUrl);
            }
        }

        if (total > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return total;
    }

    private async Task<int> SyncFeedAsync(RssFeedSource feed, int maxItems, CancellationToken cancellationToken)
    {
        var items = await FetchFeedItemsAsync(feed.Key, feed.FeedUrl, maxItems, cancellationToken);
        if (items.Count == 0)
            return 0;

        var externalIds = items.Select(i => i.ExternalId).ToList();
        var existing = await _context.News
            .Where(n => n.ExternalId != null && externalIds.Contains(n.ExternalId))
            .ToDictionaryAsync(n => n.ExternalId!, cancellationToken);

        var addedOrUpdated = 0;

        foreach (var item in items)
        {
            if (existing.TryGetValue(item.ExternalId, out var news))
            {
                news.Title = item.Title;
                news.Content = item.Content;
                news.Category = feed.Category;
                news.SourceUrl = item.SourceUrl;
                news.SourceName = feed.SourceName;
                news.ImageUrl = item.ImageUrl;
                news.CreatedDate = item.Published;
                news.Type = feed.Type;
                news.IsActive = true;
            }
            else
            {
                _context.News.Add(new News
                {
                    Title = item.Title,
                    Content = item.Content,
                    Category = feed.Category,
                    Type = feed.Type,
                    CreatedDate = item.Published,
                    IsActive = true,
                    SourceUrl = item.SourceUrl,
                    SourceName = feed.SourceName,
                    ImageUrl = item.ImageUrl,
                    ExternalId = item.ExternalId
                });
            }

            addedOrUpdated++;
        }

        return addedOrUpdated;
    }

    private async Task<IReadOnlyList<RssArticle>> FetchFeedItemsAsync(
        string feedKey,
        string feedUrl,
        int maxItems,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("RssNews");
        await using var stream = await client.GetStreamAsync(feedUrl, cancellationToken);

        using var reader = XmlReader.Create(stream, new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Prohibit });
        var syndicationFeed = SyndicationFeed.Load(reader);

        var articles = new List<RssArticle>();

        foreach (var item in syndicationFeed.Items.Take(maxItems))
        {
            var link = item.Links.FirstOrDefault(l => l.Uri != null)?.Uri?.ToString();
            if (string.IsNullOrWhiteSpace(link))
                continue;

            var title = StripHtml(item.Title?.Text)?.Trim();
            if (string.IsNullOrWhiteSpace(title))
                continue;

            var rawBody = item.Summary?.Text ?? item.Content?.ToString() ?? string.Empty;
            var description = StripHtml(rawBody);
            if (description.Length > 600)
                description = description[..597] + "...";

            var published = item.PublishDate.UtcDateTime;
            if (published.Year < 2000)
                published = DateTime.UtcNow;

            var id = item.Id?.Trim();
            if (string.IsNullOrWhiteSpace(id))
                id = link;

            articles.Add(new RssArticle(
                ExternalId: $"rss:{feedKey}:{id}",
                Title: title,
                Content: string.IsNullOrWhiteSpace(description)
                    ? "Read the full story at the source."
                    : description,
                SourceUrl: link,
                ImageUrl: ExtractImageUrl(rawBody),
                Published: published));
        }

        return articles;
    }

    private static string? ExtractImageUrl(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return null;

        var match = Regex.Match(html, """<img[^>]+src=["']([^"']+)["']""", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string StripHtml(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var text = Regex.Replace(value, "<[^>]+>", " ");
        text = Regex.Replace(text, @"\s+", " ");
        return System.Net.WebUtility.HtmlDecode(text).Trim();
    }

    private sealed record RssArticle(
        string ExternalId,
        string Title,
        string Content,
        string SourceUrl,
        string? ImageUrl,
        DateTime Published);
}
