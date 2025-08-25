using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RibbitReels.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyLeaf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Leafs");

            migrationBuilder.AddColumn<string>(
                name: "VideoContentType",
                table: "Leafs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "VideoData",
                table: "Leafs",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoFileName",
                table: "Leafs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Leafs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoContentType",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "VideoData",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "VideoFileName",
                table: "Leafs");

            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Leafs");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Leafs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
