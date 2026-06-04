using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_project_DVD_Shop.Migrations
{
    /// <inheritdoc />
    public partial class AddForumChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "ForumPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Existing thread-style posts become Q&A questions/answers
            migrationBuilder.Sql("UPDATE ForumPosts SET Channel = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "ForumPosts");
        }
    }
}
