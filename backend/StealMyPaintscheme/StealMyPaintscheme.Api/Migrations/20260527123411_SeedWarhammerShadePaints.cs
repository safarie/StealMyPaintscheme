using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerShadePaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Agrax Earthshade (Gloss)", "Shade", "Warhammer", "212, 202, 186" },
                    { "Agrax Earthshade", "Shade", "Warhammer", "52, 32, 17" },
                    { "Athonian Camoshade", "Shade", "Warhammer", "40, 39, 19" },
                    { "Biel-Tan Green", "Shade", "Warhammer", "16, 55, 36" },
                    { "Carroburg Crimson", "Shade", "Warhammer", "57, 11, 14" },
                    { "Casandora Yellow", "Shade", "Warhammer", "233, 135, 47" },
                    { "Coelia Greenshade", "Shade", "Warhammer", "14, 58, 55" },
                    { "Cryptek Armourshade (Gloss)", "Shade", "Warhammer", "242, 229, 228" },
                    { "Druchii Violet", "Shade", "Warhammer", "45, 11, 48" },
                    { "Drakenhof Nightshade", "Shade", "Warhammer", "8, 24, 39" },
                    { "Fuegan Orange", "Shade", "Warhammer", "117, 36, 4" },
                    { "Nuln Oil", "Shade", "Warhammer", "24, 24, 24" },
                    { "Nuln Oil (Gloss)", "Shade", "Warhammer", "203, 199, 196" },
                    { "Reikland Fleshshade", "Shade", "Warhammer", "62, 30, 10" },
                    { "Reikland Fleshshade (Gloss)", "Shade", "Warhammer", "239, 196, 170" },
                    { "Seraphim Sepia", "Shade", "Warhammer", "57, 37, 10" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
