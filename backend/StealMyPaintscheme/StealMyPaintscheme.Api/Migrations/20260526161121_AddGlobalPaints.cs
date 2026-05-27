using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaintDatabase",
                table: "PaintDatabase");

            migrationBuilder.RenameTable(
                name: "PaintDatabase",
                newName: "PaintDatabases");

            migrationBuilder.RenameColumn(
                name: "RGB",
                table: "PaintDatabases",
                newName: "Maker");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaintDatabases",
                table: "PaintDatabases",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GlobalPaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Maker = table.Column<string>(type: "text", nullable: false),
                    RGB = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalPaints", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalPaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaintDatabases",
                table: "PaintDatabases");

            migrationBuilder.RenameTable(
                name: "PaintDatabases",
                newName: "PaintDatabase");

            migrationBuilder.RenameColumn(
                name: "Maker",
                table: "PaintDatabase",
                newName: "RGB");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaintDatabase",
                table: "PaintDatabase",
                column: "Id");
        }
    }
}
