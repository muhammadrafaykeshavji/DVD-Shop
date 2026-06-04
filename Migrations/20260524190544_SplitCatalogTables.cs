using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_project_DVD_Shop.Migrations;

/// <inheritdoc />
public partial class SplitCatalogTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "GameStudioId",
            table: "Games",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "CategoryId",
            table: "Games",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "ReleaseDate",
            table: "Games",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CoverImageUrl",
            table: "Games",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "FilmStudioId",
            table: "Movies",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "CategoryId",
            table: "Movies",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "ReleaseDate",
            table: "Movies",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CoverImageUrl",
            table: "Movies",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "GameId",
            table: "Products",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "MovieId",
            table: "Products",
            type: "int",
            nullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "AlbumId",
            table: "Products",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.Sql("""
            UPDATE g
            SET g.GameStudioId = COALESCE(a.GameStudioId, (SELECT TOP 1 GameStudioId FROM GameStudios ORDER BY GameStudioId)),
                g.CategoryId = a.CategoryId,
                g.ReleaseDate = a.ReleaseDate,
                g.CoverImageUrl = COALESCE(a.CoverImageUrl, g.ImageUrl),
                g.Description = COALESCE(g.Description, a.Description),
                g.Title = a.Title
            FROM Games g
            INNER JOIN Albums a ON g.AlbumId = a.AlbumId
            WHERE a.MediaType = 1;

            INSERT INTO Games (Title, AlbumId, GameStudioId, CategoryId, ReleaseDate, CoverImageUrl, ImageUrl, Description)
            SELECT a.Title, a.AlbumId,
                COALESCE(a.GameStudioId, (SELECT TOP 1 GameStudioId FROM GameStudios ORDER BY GameStudioId)),
                a.CategoryId, a.ReleaseDate, a.CoverImageUrl, a.CoverImageUrl, a.Description
            FROM Albums a
            WHERE a.MediaType = 1
              AND NOT EXISTS (SELECT 1 FROM Games g WHERE g.AlbumId = a.AlbumId);

            UPDATE m
            SET m.FilmStudioId = COALESCE(a.FilmStudioId, (SELECT TOP 1 FilmStudioId FROM FilmStudios ORDER BY FilmStudioId)),
                m.CategoryId = a.CategoryId,
                m.ReleaseDate = a.ReleaseDate,
                m.CoverImageUrl = COALESCE(a.CoverImageUrl, m.ImageUrl),
                m.Description = COALESCE(m.Description, a.Description),
                m.Title = a.Title
            FROM Movies m
            INNER JOIN Albums a ON m.AlbumId = a.AlbumId
            WHERE a.MediaType = 2;

            INSERT INTO Movies (Title, AlbumId, FilmStudioId, CategoryId, ReleaseDate, CoverImageUrl, ImageUrl, Description)
            SELECT a.Title, a.AlbumId,
                COALESCE(a.FilmStudioId, (SELECT TOP 1 FilmStudioId FROM FilmStudios ORDER BY FilmStudioId)),
                a.CategoryId, a.ReleaseDate, a.CoverImageUrl, a.CoverImageUrl, a.Description
            FROM Albums a
            WHERE a.MediaType = 2
              AND NOT EXISTS (SELECT 1 FROM Movies m WHERE m.AlbumId = a.AlbumId);

            UPDATE p SET p.GameId = g.GameId, p.AlbumId = NULL
            FROM Products p
            INNER JOIN Games g ON g.AlbumId = p.AlbumId
            INNER JOIN Albums a ON a.AlbumId = p.AlbumId
            WHERE a.MediaType = 1;

            UPDATE p SET p.MovieId = m.MovieId, p.AlbumId = NULL
            FROM Products p
            INNER JOIN Movies m ON m.AlbumId = p.AlbumId
            INNER JOIN Albums a ON a.AlbumId = p.AlbumId
            WHERE a.MediaType = 2;
            """);

        migrationBuilder.DropForeignKey(
            name: "FK_Games_Albums_AlbumId",
            table: "Games");

        migrationBuilder.DropForeignKey(
            name: "FK_Movies_Albums_AlbumId",
            table: "Movies");

        migrationBuilder.DropIndex(
            name: "IX_Games_AlbumId",
            table: "Games");

        migrationBuilder.DropIndex(
            name: "IX_Movies_AlbumId",
            table: "Movies");

        migrationBuilder.DropColumn(
            name: "AlbumId",
            table: "Games");

        migrationBuilder.DropColumn(
            name: "AlbumId",
            table: "Movies");

        migrationBuilder.Sql("DELETE FROM Albums WHERE MediaType IN (1, 2);");

        migrationBuilder.DropForeignKey(
            name: "FK_Albums_FilmStudios_FilmStudioId",
            table: "Albums");

        migrationBuilder.DropForeignKey(
            name: "FK_Albums_GameStudios_GameStudioId",
            table: "Albums");

        migrationBuilder.DropIndex(
            name: "IX_Albums_FilmStudioId",
            table: "Albums");

        migrationBuilder.DropIndex(
            name: "IX_Albums_GameStudioId",
            table: "Albums");

        migrationBuilder.DropColumn(
            name: "FilmStudioId",
            table: "Albums");

        migrationBuilder.DropColumn(
            name: "GameStudioId",
            table: "Albums");

        migrationBuilder.DropColumn(
            name: "MediaType",
            table: "Albums");

        migrationBuilder.AlterColumn<int>(
            name: "GameStudioId",
            table: "Games",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "CategoryId",
            table: "Games",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "ReleaseDate",
            table: "Games",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
            oldClrType: typeof(DateTime),
            oldType: "datetime2",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "FilmStudioId",
            table: "Movies",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "CategoryId",
            table: "Movies",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
            name: "ReleaseDate",
            table: "Movies",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
            oldClrType: typeof(DateTime),
            oldType: "datetime2",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Products_GameId",
            table: "Products",
            column: "GameId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_MovieId",
            table: "Products",
            column: "MovieId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_GameStudioId",
            table: "Games",
            column: "GameStudioId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_CategoryId",
            table: "Games",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Movies_FilmStudioId",
            table: "Movies",
            column: "FilmStudioId");

        migrationBuilder.CreateIndex(
            name: "IX_Movies_CategoryId",
            table: "Movies",
            column: "CategoryId");

        migrationBuilder.AddForeignKey(
            name: "FK_Games_Categories_CategoryId",
            table: "Games",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "CategoryId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Games_GameStudios_GameStudioId",
            table: "Games",
            column: "GameStudioId",
            principalTable: "GameStudios",
            principalColumn: "GameStudioId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Movies_Categories_CategoryId",
            table: "Movies",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "CategoryId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Movies_FilmStudios_FilmStudioId",
            table: "Movies",
            column: "FilmStudioId",
            principalTable: "FilmStudios",
            principalColumn: "FilmStudioId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Products_Games_GameId",
            table: "Products",
            column: "GameId",
            principalTable: "Games",
            principalColumn: "GameId",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Products_Movies_MovieId",
            table: "Products",
            column: "MovieId",
            principalTable: "Movies",
            principalColumn: "MovieId",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException("SplitCatalogTables cannot be reverted automatically.");
    }
}
