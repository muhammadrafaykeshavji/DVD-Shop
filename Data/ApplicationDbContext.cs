using E_project_DVD_Shop.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<MusicStudio> MusicStudios => Set<MusicStudio>();
    public DbSet<GameStudio> GameStudios => Set<GameStudio>();
    public DbSet<FilmStudio> FilmStudios => Set<FilmStudio>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<News> News => Set<News>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Producer> Producers => Set<Producer>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Advertisement> Advertisements => Set<Advertisement>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();
    public DbSet<PurchasingInvoice> PurchasingInvoices => Set<PurchasingInvoice>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasKey(c => c.CategoryId);
        builder.Entity<Artist>().HasKey(a => a.ArtistId);
        builder.Entity<MusicStudio>().HasKey(s => s.MusicStudioId);
        builder.Entity<GameStudio>().HasKey(s => s.GameStudioId);
        builder.Entity<FilmStudio>().HasKey(s => s.FilmStudioId);
        builder.Entity<Album>().HasKey(a => a.AlbumId);
        builder.Entity<Product>().HasKey(p => p.ProductId);
        builder.Entity<Song>().HasKey(s => s.SongId);
        builder.Entity<Game>().HasKey(g => g.GameId);
        builder.Entity<Movie>().HasKey(m => m.MovieId);
        builder.Entity<News>(entity =>
        {
            entity.HasKey(n => n.NewsId);
            entity.HasIndex(n => n.ExternalId)
                .IsUnique()
                .HasFilter("[ExternalId] IS NOT NULL");
        });
        builder.Entity<Order>().HasKey(o => o.OrderId);
        builder.Entity<OrderItem>().HasKey(oi => oi.OrderItemId);
        builder.Entity<Review>().HasKey(r => r.ReviewId);
        builder.Entity<Feedback>().HasKey(f => f.FeedbackId);
        builder.Entity<Supplier>().HasKey(s => s.SupplierId);
        builder.Entity<Producer>().HasKey(p => p.ProducerId);
        builder.Entity<Permission>().HasKey(p => p.PermissionId);
        builder.Entity<Wishlist>().HasKey(w => w.WishlistId);
        builder.Entity<ForumPost>().HasKey(f => f.PostId);
        builder.Entity<PurchasingInvoice>().HasKey(p => p.PurchasingInvoiceId);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Balance).HasPrecision(18, 2);
        });

        builder.Entity<Album>(entity =>
        {
            entity.HasOne(a => a.Artist)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.MusicStudio)
                .WithMany(s => s.Albums)
                .HasForeignKey(a => a.MusicStudioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Category)
                .WithMany(c => c.Albums)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasPrecision(18, 2);

            entity.HasOne(p => p.Album)
                .WithMany(a => a.Products)
                .HasForeignKey(p => p.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Game)
                .WithMany(g => g.Products)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Movie)
                .WithMany(m => m.Products)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Producer)
                .WithMany(pr => pr.Products)
                .HasForeignKey(p => p.ProducerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Song>(entity =>
        {
            entity.HasOne(s => s.Album)
                .WithMany(a => a.Songs)
                .HasForeignKey(s => s.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Game>(entity =>
        {
            entity.HasOne(g => g.GameStudio)
                .WithMany(s => s.Games)
                .HasForeignKey(g => g.GameStudioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.Category)
                .WithMany(c => c.Games)
                .HasForeignKey(g => g.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Movie>(entity =>
        {
            entity.HasOne(m => m.FilmStudio)
                .WithMany(s => s.Movies)
                .HasForeignKey(m => m.FilmStudioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Order>(entity =>
        {
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            entity.Property(o => o.DiscountApplied).HasPrecision(18, 2);

            entity.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
        });

        builder.Entity<Feedback>(entity =>
        {
            entity.HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(a => a.AdId);
        });

        builder.Entity<Promotion>(entity =>
        {
            entity.HasKey(p => p.PromotionId);
            entity.Property(p => p.DiscountPercent).HasPrecision(5, 2);
        });

        builder.Entity<Permission>(entity =>
        {
            entity.HasOne(p => p.Admin)
                .WithMany(u => u.Permissions)
                .HasForeignKey(p => p.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.AdminId, p.ModuleName }).IsUnique();
        });

        builder.Entity<Wishlist>(entity =>
        {
            entity.HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(w => w.Product)
                .WithMany(p => p.Wishlists)
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        });

        builder.Entity<ForumPost>(entity =>
        {
            entity.HasOne(fp => fp.User)
                .WithMany(u => u.ForumPosts)
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(fp => fp.ParentPost)
                .WithMany(fp => fp.Replies)
                .HasForeignKey(fp => fp.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PurchasingInvoice>(entity =>
        {
            entity.Property(pi => pi.Amount).HasPrecision(18, 2);

            entity.HasOne(pi => pi.Supplier)
                .WithMany(s => s.PurchasingInvoices)
                .HasForeignKey(pi => pi.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
