using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovingSizeOptionNameToMasterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SizeOptions");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SizeOptionMasterIdentifiers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 31, 9, 2, 22, 733, DateTimeKind.Utc).AddTicks(5410));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "SizeOptionMasterIdentifiers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SizeOptions",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 21, 21, 24, 43, 608, DateTimeKind.Utc).AddTicks(2680));
        }
    }
}
