using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RibbitReels.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeafModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBranchAssignment_UserId",
                table: "UserBranchAssignment");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Leafs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Leafs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Leafs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Leafs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Leafs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Leafs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAssignment_UserId_BranchId",
                table: "UserBranchAssignment",
                columns: new[] { "UserId", "BranchId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBranchAssignment_UserId_BranchId",
                table: "UserBranchAssignment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Leafs");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Leafs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchAssignment_UserId",
                table: "UserBranchAssignment",
                column: "UserId");
        }
    }
}
