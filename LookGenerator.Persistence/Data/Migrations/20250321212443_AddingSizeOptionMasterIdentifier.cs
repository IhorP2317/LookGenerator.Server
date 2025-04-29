using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingSizeOptionMasterIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasterSizeIdentifiers_SizeOptions_SizeOptionId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariations_MasterSizeIdentifiers_MasterSizeId",
                table: "ProductVariations");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariations_MasterSizeId",
                table: "ProductVariations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MasterSizeIdentifiers",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_MasterSizeIdentifiers_SizeOptionId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "MasterSizeId",
                table: "ProductVariations");

            migrationBuilder.RenameColumn(
                name: "SizeOptionId",
                table: "MasterSizeIdentifiers",
                newName: "Id");

            migrationBuilder.AddColumn<Guid>(
                name: "MasterSizeIdentifierId",
                table: "ProductVariations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MasterSizeIdentifiers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "MasterSizeIdentifiers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "MasterSizeIdentifiers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "MasterSizeIdentifiers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "MasterSizeIdentifiers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterSizeIdentifiers",
                table: "MasterSizeIdentifiers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SizeOptionMasterIdentifiers",
                columns: table => new
                {
                    SizeOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MasterIdentifierId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeOptionMasterIdentifiers", x => new { x.SizeOptionId, x.MasterIdentifierId });
                    table.ForeignKey(
                        name: "FK_SizeOptionMasterIdentifiers_MasterSizeIdentifiers_MasterIde~",
                        column: x => x.MasterIdentifierId,
                        principalTable: "MasterSizeIdentifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SizeOptionMasterIdentifiers_SizeOptions_SizeOptionId",
                        column: x => x.SizeOptionId,
                        principalTable: "SizeOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 21, 21, 24, 43, 608, DateTimeKind.Utc).AddTicks(2680));

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariations_MasterSizeIdentifierId",
                table: "ProductVariations",
                column: "MasterSizeIdentifierId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSizeIdentifiers_CreatorId",
                table: "MasterSizeIdentifiers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SizeOptionMasterIdentifiers_MasterIdentifierId",
                table: "SizeOptionMasterIdentifiers",
                column: "MasterIdentifierId");

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSizeIdentifiers_Users_CreatorId",
                table: "MasterSizeIdentifiers",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariations_MasterSizeIdentifiers_MasterSizeIdentifie~",
                table: "ProductVariations",
                column: "MasterSizeIdentifierId",
                principalTable: "MasterSizeIdentifiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasterSizeIdentifiers_Users_CreatorId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariations_MasterSizeIdentifiers_MasterSizeIdentifie~",
                table: "ProductVariations");

            migrationBuilder.DropTable(
                name: "SizeOptionMasterIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariations_MasterSizeIdentifierId",
                table: "ProductVariations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MasterSizeIdentifiers",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_MasterSizeIdentifiers_CreatorId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "MasterSizeIdentifierId",
                table: "ProductVariations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "MasterSizeIdentifiers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MasterSizeIdentifiers",
                newName: "SizeOptionId");

            migrationBuilder.AddColumn<string>(
                name: "MasterSizeId",
                table: "ProductVariations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterSizeIdentifiers",
                table: "MasterSizeIdentifiers",
                column: "Identifier");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"),
                column: "CreatedAt",
                value: new DateTime(2025, 3, 18, 20, 38, 6, 570, DateTimeKind.Utc).AddTicks(2560));

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariations_MasterSizeId",
                table: "ProductVariations",
                column: "MasterSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSizeIdentifiers_SizeOptionId",
                table: "MasterSizeIdentifiers",
                column: "SizeOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSizeIdentifiers_SizeOptions_SizeOptionId",
                table: "MasterSizeIdentifiers",
                column: "SizeOptionId",
                principalTable: "SizeOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariations_MasterSizeIdentifiers_MasterSizeId",
                table: "ProductVariations",
                column: "MasterSizeId",
                principalTable: "MasterSizeIdentifiers",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
