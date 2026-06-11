using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalPaintIdToStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GlobalPaintId",
                table: "Steps",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Steps_GlobalPaintId",
                table: "Steps",
                column: "GlobalPaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_GlobalPaints_GlobalPaintId",
                table: "Steps",
                column: "GlobalPaintId",
                principalTable: "GlobalPaints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_GlobalPaints_GlobalPaintId",
                table: "Steps");

            migrationBuilder.DropIndex(
                name: "IX_Steps_GlobalPaintId",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "GlobalPaintId",
                table: "Steps");
        }
    }
}
