using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LookGenerator.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovingRedundantUserConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Looks_Users_CreatorId",
                table: "Looks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedBy",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"));

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Looks",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Looks_CreatorId",
                table: "Looks",
                newName: "IX_Looks_UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatorId",
                table: "Users",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Looks_Users_UserId",
                table: "Looks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatorId",
                table: "Users",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Looks_Users_UserId",
                table: "Looks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatorId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Looks",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Looks_UserId",
                table: "Looks",
                newName: "IX_Looks_CreatorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "EmailConfirmed", "ModifiedAt", "ModifiedBy", "Role", "UserName" },
                values: new object[] { new Guid("226b1dad-0065-44c6-acef-93186e7cd0f2"), new DateTime(2025, 4, 29, 20, 41, 15, 167, DateTimeKind.Utc).AddTicks(7730), null, "mrsplash2356@gmail.com", true, null, null, "Admin", "Ihor" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Looks_Users_CreatorId",
                table: "Looks",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
