using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStolenSuffixFromName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"PaintSchemes\" SET \"Name\" = REPLACE(\"Name\", ' (Stolen)', '') WHERE \"Name\" LIKE '% (Stolen)'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"PaintSchemes\" SET \"Name\" = \"Name\" || ' (Stolen)' WHERE \"IsStolen\" = true AND \"Name\" NOT LIKE '% (Stolen)'");
        }
    }
}
