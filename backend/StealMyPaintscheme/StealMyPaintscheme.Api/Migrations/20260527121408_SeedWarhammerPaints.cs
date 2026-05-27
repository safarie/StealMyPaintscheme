using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Chaos Black", "Spray", "Warhammer", "0, 0, 0" },
                    { "Corax White", "Spray", "Warhammer", "229, 229, 229" },
                    { "Death Guard Green", "Spray", "Warhammer", "85, 98, 41" },
                    { "Grey Seer", "Spray", "Warhammer", "162, 165, 167" },
                    { "Leadbelcher (Metal)", "Spray", "Warhammer", "87, 91, 93" },
                    { "Macragge Blue", "Spray", "Warhammer", "15, 61, 124" },
                    { "Mechanicus Standard Grey", "Spray", "Warhammer", "57, 72, 74" },
                    { "Mephiston Red", "Spray", "Warhammer", "150, 12, 9" },
                    { "Munitorum Varnish", "Spray", "Warhammer", "229, 229, 229" },
                    { "Retributor Armour (Metal)", "Spray", "Warhammer", "193, 142, 61" },
                    { "Wraithbone", "Spray", "Warhammer", "219, 209, 178" },
                    { "Zandri Dust", "Spray", "Warhammer", "152, 142, 86" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GlobalPaints",
                keyColumn: "Maker",
                keyValue: "Warhammer");
        }
    }
}
