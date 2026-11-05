using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class HashAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ImageUrl is al toegevoegd in AddImageUrlToPaintScheme, dus alleen toevoegen als hij nog ontbreekt
            migrationBuilder.Sql(@"ALTER TABLE ""PaintSchemes"" ADD COLUMN IF NOT EXISTS ""ImageUrl"" text NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "PaintSchemes");
        }
    }
}
