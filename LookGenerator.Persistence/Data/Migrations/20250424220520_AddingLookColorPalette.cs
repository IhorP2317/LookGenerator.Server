using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingLookColorPalette : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorPalette",
                table: "Looks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 24, 22, 5, 19, 966, DateTimeKind.Utc).AddTicks(8720));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorPalette",
                table: "Looks");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 22, 12, 56, 47, 942, DateTimeKind.Utc).AddTicks(3830));
        }
    }
}
