using E_project_DVD_Shop.Models.Entities;
using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_project_DVD_Shop.Data;

/// <summary>Runs migrations and seeds roles, core users, and optional heavy test data.</summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await context.Database.MigrateAsync();

        var seedHeavy = configuration.GetValue("SeedHeavyTestData", true);
        if (seedHeavy)
            await HeavyTestDataSeeder.SeedAsync(context, userManager, roleManager);

        await HeavyTestDataSeeder.EnforceCategoryCatalogAsync(context);
        await HeavyTestDataSeeder.EnforceGameStudioCatalogAsync(context);
        await HeavyTestDataSeeder.EnforceFilmStudioCatalogAsync(context);

        await ImagePathRepair.RepairAsync(context);
        await CoverImageRepair.RepairAsync(context);
        await MediaLinksRepair.RepairAsync(context);
        await ForumEncodingRepair.RepairAsync(context);
        await ForumQnaSeedRepair.EnsureAsync(context, userManager);
        await ForumGeneralSeedRepair.EnsureAsync(context, userManager);

        if (configuration.GetValue("RssNews:SyncOnStartup", true))
        {
            var rssSync = scope.ServiceProvider.GetRequiredService<RssNewsSyncService>();
            await rssSync.SyncAllAsync();
        }

        if (seedHeavy)
            return;

        // Minimal seed when heavy test data is disabled
        foreach (var role in new[] { "Member", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await context.Categories.AnyAsync())
            return;

        await HeavyTestDataSeeder.SeedAsync(context, userManager, roleManager);
    }
}
