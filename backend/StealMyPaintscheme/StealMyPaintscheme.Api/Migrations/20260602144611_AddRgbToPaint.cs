using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRgbToPaint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RGB",
                table: "Paints",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RGB",
                table: "Paints");
        }
    }
}
