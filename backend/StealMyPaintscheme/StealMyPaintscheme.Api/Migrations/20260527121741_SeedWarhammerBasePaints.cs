using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerBasePaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Abaddon Black", "Base", "Warhammer", "0, 0, 0" },
                    { "Averland Sunset", "Base", "Warhammer", "251, 184, 28" },
                    { "Balthasar Gold (Metal)", "Base", "Warhammer", "106, 69, 48" },
                    { "Barak-Nar Burgundy", "Base", "Warhammer", "69, 22, 54" },
                    { "Bugman‘s Glow", "Base", "Warhammer", "128, 76, 67" },
                    { "Caledor Sky", "Base", "Warhammer", "54, 102, 153" },
                    { "Caliban Green", "Base", "Warhammer", "0, 61, 21" },
                    { "Celestra Grey", "Base", "Warhammer", "139, 163, 163" },
                    { "Catachan Fleshtone", "Base", "Warhammer", "68, 43, 37" },
                    { "Castellan Green", "Base", "Warhammer", "38, 71, 21" },
                    { "Corax White", "Base", "Warhammer", "255, 255, 255" },
                    { "Corvus Black", "Base", "Warhammer", "23, 19, 20" },
                    { "Daemonette Hide", "Base", "Warhammer", "101, 95, 129" },
                    { "Death Guard Green", "Base", "Warhammer", "109, 119, 77" },
                    { "Death Korps Drab", "Base", "Warhammer", "61, 69, 57" },
                    { "Deathworld Forest", "Base", "Warhammer", "85, 98, 41" },
                    { "Dryad Bark", "Base", "Warhammer", "43, 42, 36" },
                    { "Gal Vorbak Red", "Base", "Warhammer", "75, 33, 60" },
                    { "Grey Knights Steel (Metal)", "Base", "Warhammer", "131, 147, 157" },
                    { "Grey Seer", "Base", "Warhammer", "162, 165, 167" },
                    { "Incubi Darkness", "Base", "Warhammer", "8, 46, 50" },
                    { "Ionrach Skin", "Base", "Warhammer", "151, 163, 132" },
                    { "Iron Hands Steel (Metal)", "Base", "Warhammer", "121, 114, 108" },
                    { "Iron Warriors (Metal)", "Base", "Warhammer", "76, 73, 71" },
                    { "Jokaero Orange", "Base", "Warhammer", "237, 56, 20" },
                    { "Kantor Blue", "Base", "Warhammer", "2, 19, 78" },
                    { "Khorne Red", "Base", "Warhammer", "101, 0, 1" },
                    { "Leadbelcher (Metal)", "Base", "Warhammer", "88, 92, 94" },
                    { "Lupercal Green", "Base", "Warhammer", "0, 44, 43" },
                    { "Macragge Blue", "Base", "Warhammer", "15, 61, 124" },
                    { "Mechanicus Standard Grey", "Base", "Warhammer", "57, 72, 74" },
                    { "Mephiston Red", "Base", "Warhammer", "150, 12, 9" },
                    { "Morghast Bone", "Base", "Warhammer", "192, 169, 115" },
                    { "Mournfang Brown", "Base", "Warhammer", "73, 15, 6" },
                    { "Naggaroth Night", "Base", "Warhammer", "59, 43, 80" },
                    { "Night Lords Blue", "Base", "Warhammer", "0, 43, 92" },
                    { "Nocturne Green", "Base", "Warhammer", "22, 42, 41" },
                    { "Phoenician Purple", "Base", "Warhammer", "68, 0, 82" },
                    { "Rakarth Flesh", "Base", "Warhammer", "156, 153, 141" },
                    { "Ratskin Flesh", "Base", "Warhammer", "168, 102, 72" },
                    { "Retributor Armour (Metal)", "Base", "Warhammer", "200, 150, 64" },
                    { "Rhinox Hide", "Base", "Warhammer", "70, 47, 48" },
                    { "Runelord Brass", "Base", "Warhammer", "69, 54, 41" },
                    { "Screamer Pink", "Base", "Warhammer", "122, 14, 68" },
                    { "Screaming Bell (Metal)", "Base", "Warhammer", "163, 94, 61" },
                    { "Steel Legion Drab", "Base", "Warhammer", "88, 78, 45" },
                    { "Stegadon Scale Green", "Base", "Warhammer", "6, 69, 93" },
                    { "The Fang", "Base", "Warhammer", "64, 91, 113" },
                    { "Thousand Sons Blue", "Base", "Warhammer", "0, 80, 111" },
                    { "Waaagh! Flesh", "Base", "Warhammer", "11, 59, 54" },
                    { "Warplock Bronze (Metal)", "Base", "Warhammer", "124, 58, 40" },
                    { "Wraithbone", "Base", "Warhammer", "219, 209, 178" },
                    { "XV-88", "Base", "Warhammer", "108, 72, 17" },
                    { "Zandri Dust", "Base", "Warhammer", "152, 142, 86" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
