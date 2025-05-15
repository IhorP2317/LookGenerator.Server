using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovingReactionContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Reactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Reactions",
                type: "text",
                nullable: true);
        }
    }
}
