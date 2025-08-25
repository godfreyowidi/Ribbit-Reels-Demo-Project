using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RibbitReels.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBlobPathUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoUrl",
                table: "Leafs",
                newName: "VideoBlobPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoBlobPath",
                table: "Leafs",
                newName: "VideoUrl");
        }
    }
}
