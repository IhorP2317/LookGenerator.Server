using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingColours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colour",
                table: "ProductItems");

            migrationBuilder.AddColumn<int>(
                name: "BodyZone",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ColourId",
                table: "ProductItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Colours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colours", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 6, 18, 30, 54, 54, DateTimeKind.Utc).AddTicks(440));

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_ColourId",
                table: "ProductItems",
                column: "ColourId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Colours_ColourId",
                table: "ProductItems",
                column: "ColourId",
                principalTable: "Colours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Colours_ColourId",
                table: "ProductItems");

            migrationBuilder.DropTable(
                name: "Colours");

            migrationBuilder.DropIndex(
                name: "IX_ProductItems_ColourId",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "BodyZone",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ColourId",
                table: "ProductItems");

            migrationBuilder.AddColumn<string>(
                name: "Colour",
                table: "ProductItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 6, 15, 11, 58, 950, DateTimeKind.Utc).AddTicks(9840));
        }
    }
}
