using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_project_DVD_Shop.Migrations
{
    /// <inheritdoc />
    public partial class SplitStudioTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "Albums",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FilmStudioId",
                table: "Albums",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameStudioId",
                table: "Albums",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MusicStudioId",
                table: "Albums",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilmStudios",
                columns: table => new
                {
                    FilmStudioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmStudios", x => x.FilmStudioId);
                });

            migrationBuilder.CreateTable(
                name: "GameStudios",
                columns: table => new
                {
                    GameStudioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStudios", x => x.GameStudioId);
                });

            migrationBuilder.CreateTable(
                name: "MusicStudios",
                columns: table => new
                {
                    MusicStudioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicStudios", x => x.MusicStudioId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_FilmStudioId",
                table: "Albums",
                column: "FilmStudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_GameStudioId",
                table: "Albums",
                column: "GameStudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_MusicStudioId",
                table: "Albums",
                column: "MusicStudioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_FilmStudios_FilmStudioId",
                table: "Albums",
                column: "FilmStudioId",
                principalTable: "FilmStudios",
                principalColumn: "FilmStudioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_GameStudios_GameStudioId",
                table: "Albums",
                column: "GameStudioId",
                principalTable: "GameStudios",
                principalColumn: "GameStudioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Albums_MusicStudios_MusicStudioId",
                table: "Albums",
                column: "MusicStudioId",
                principalTable: "MusicStudios",
                principalColumn: "MusicStudioId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Albums_FilmStudios_FilmStudioId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Albums_GameStudios_GameStudioId",
                table: "Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_Albums_MusicStudios_MusicStudioId",
                table: "Albums");

            migrationBuilder.DropTable(
                name: "FilmStudios");

            migrationBuilder.DropTable(
                name: "GameStudios");

            migrationBuilder.DropTable(
                name: "MusicStudios");

            migrationBuilder.DropIndex(
                name: "IX_Albums_FilmStudioId",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_Albums_GameStudioId",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_Albums_MusicStudioId",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "FilmStudioId",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "GameStudioId",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "MusicStudioId",
                table: "Albums");

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "Albums",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
