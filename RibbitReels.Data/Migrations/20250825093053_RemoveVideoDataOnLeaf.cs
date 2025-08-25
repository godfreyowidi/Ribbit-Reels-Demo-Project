using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RibbitReels.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVideoDataOnLeaf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoData",
                table: "Leafs");

            migrationBuilder.AlterColumn<string>(
                name: "VideoContentType",
                table: "Leafs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Leafs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Leafs");

            migrationBuilder.AlterColumn<string>(
                name: "VideoContentType",
                table: "Leafs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "VideoData",
                table: "Leafs",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
