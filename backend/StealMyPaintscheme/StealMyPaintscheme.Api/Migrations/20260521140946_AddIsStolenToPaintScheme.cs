using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsStolenToPaintScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalPaintSchemeId",
                table: "PaintSchemes");

            migrationBuilder.AddColumn<bool>(
                name: "IsStolen",
                table: "PaintSchemes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStolen",
                table: "PaintSchemes");

            migrationBuilder.AddColumn<int>(
                name: "OriginalPaintSchemeId",
                table: "PaintSchemes",
                type: "integer",
                nullable: true);
        }
    }
}
