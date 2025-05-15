using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixingBaseEntitiesScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Looks_Users_CreatedBy",
                table: "Looks");

            migrationBuilder.DropForeignKey(
                name: "FK_Looks_Users_UserId",
                table: "Looks");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSizeIdentifiers_Users_CreatorId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatorId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatorId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_MasterSizeIdentifiers_CreatorId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_Looks_UserId",
                table: "Looks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Looks");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MasterSizeIdentifiers",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariations_Size",
                table: "ProductVariations",
                column: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Description",
                table: "Products",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSizeIdentifiers_CreatedBy",
                table: "MasterSizeIdentifiers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSizeIdentifiers_Identifier",
                table: "MasterSizeIdentifiers",
                column: "Identifier");

            migrationBuilder.CreateIndex(
                name: "IX_Looks_Description",
                table: "Looks",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Looks_Name",
                table: "Looks",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Colours_Name",
                table: "Colours",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Looks_Users_CreatedBy",
                table: "Looks",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSizeIdentifiers_Users_CreatedBy",
                table: "MasterSizeIdentifiers",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Looks_Users_CreatedBy",
                table: "Looks");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterSizeIdentifiers_Users_CreatedBy",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariations_Size",
                table: "ProductVariations");

            migrationBuilder.DropIndex(
                name: "IX_Products_Description",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_MasterSizeIdentifiers_CreatedBy",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_MasterSizeIdentifiers_Identifier",
                table: "MasterSizeIdentifiers");

            migrationBuilder.DropIndex(
                name: "IX_Looks_Description",
                table: "Looks");

            migrationBuilder.DropIndex(
                name: "IX_Looks_Name",
                table: "Looks");

            migrationBuilder.DropIndex(
                name: "IX_Colours_Name",
                table: "Colours");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MasterSizeIdentifiers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "MasterSizeIdentifiers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Looks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatorId",
                table: "Users",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterSizeIdentifiers_CreatorId",
                table: "MasterSizeIdentifiers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Looks_UserId",
                table: "Looks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Looks_Users_CreatedBy",
                table: "Looks",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Looks_Users_UserId",
                table: "Looks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MasterSizeIdentifiers_Users_CreatorId",
                table: "MasterSizeIdentifiers",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatorId",
                table: "Users",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
