using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerContrastPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Aethermatic Blue", "Contrast", "Warhammer", "0, 109, 131" },
                    { "Aggaros Dunes", "Contrast", "Warhammer", "85, 76, 54" },
                    { "Akhelian Green", "Contrast", "Warhammer", "0, 67, 84" },
                    { "Apothecary White", "Contrast", "Warhammer", "143, 173, 200" },
                    { "Basilicanum Grey", "Contrast", "Warhammer", "0, 0, 0" },
                    { "Black Templar", "Contrast", "Warhammer", "0, 0, 0" },
                    { "Blood Angels Red", "Contrast", "Warhammer", "118, 8, 11" },
                    { "Creed Camo", "Contrast", "Warhammer", "0, 78, 27" },
                    { "Cygor Brown", "Contrast", "Warhammer", "47, 13, 10" },
                    { "Dark Angels Green", "Contrast", "Warhammer", "0, 0, 0" },
                    { "Darkoath Flesh", "Contrast", "Warhammer", "135, 82, 79" },
                    { "Flesh Tearers Red", "Contrast", "Warhammer", "58, 1, 0" },
                    { "Fyreslayer Flesh", "Contrast", "Warhammer", "93, 66, 49" },
                    { "Gore-grunta Fur", "Contrast", "Warhammer", "84, 57, 39" },
                    { "Gryph-charger Grey", "Contrast", "Warhammer", "0, 78, 116" },
                    { "Gryph-hound Orange", "Contrast", "Warhammer", "124, 35, 3" },
                    { "Guilliman Flesh", "Contrast", "Warhammer", "145, 45, 32" },
                    { "Iyanden Yellow", "Contrast", "Warhammer", "210, 107, 4" },
                    { "Leviadon Blue", "Contrast", "Warhammer", "0, 0, 0" },
                    { "Magos Purple", "Contrast", "Warhammer", "102, 35, 87" },
                    { "Militarum Green", "Contrast", "Warhammer", "95, 109, 0" },
                    { "Nazdreg Yellow", "Contrast", "Warhammer", "78, 55, 0" },
                    { "Ork Flesh", "Contrast", "Warhammer", "0, 78, 27" },
                    { "Plaguebearer Flesh", "Contrast", "Warhammer", "89, 107, 2" },
                    { "Shyish Purple", "Contrast", "Warhammer", "31, 1, 55" },
                    { "Skeleton Horde", "Contrast", "Warhammer", "145, 129, 88" },
                    { "Snakebite Leather", "Contrast", "Warhammer", "85, 68, 49" },
                    { "Space Wolves Grey", "Contrast", "Warhammer", "0, 44, 81" },
                    { "Talassar Blue", "Contrast", "Warhammer", "0, 44, 89" },
                    { "Terradon Turquoise", "Contrast", "Warhammer", "0, 73, 79" },
                    { "Ultramarines Blue", "Contrast", "Warhammer", "0, 10, 55" },
                    { "Volupus Pink", "Contrast", "Warhammer", "83, 0, 17" },
                    { "Warp Lightning", "Contrast", "Warhammer", "0, 78, 27" },
                    { "Wyldwood", "Contrast", "Warhammer", "56, 29, 23" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
