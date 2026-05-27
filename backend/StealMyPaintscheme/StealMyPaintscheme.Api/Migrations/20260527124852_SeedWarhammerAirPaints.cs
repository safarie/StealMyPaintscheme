using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerAirPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Abaddon Black", "Air", "Warhammer", "0, 0, 0" },
                    { "Administratum Grey", "Air", "Warhammer", "143, 150, 144" },
                    { "Air Caste Thinner", "Air", "Warhammer", "255, 255, 255" },
                    { "Angron Red (Clear)", "Air", "Warhammer", "226, 24, 35" },
                    { "Averland Sunset", "Air", "Warhammer", "251, 184, 28" },
                    { "Balthasar Gold (Metal)", "Air", "Warhammer", "78, 49, 33" },
                    { "Balor Brown", "Air", "Warhammer", "135, 84, 8" },
                    { "Baneblade Brown", "Air", "Warhammer", "143, 124, 104" },
                    { "Calgar Blue", "Air", "Warhammer", "42, 73, 127" },
                    { "Caledor Sky", "Air", "Warhammer", "54, 102, 153" },
                    { "Caliban Green", "Air", "Warhammer", "0, 61, 21" },
                    { "Calth Blue (Clear)", "Air", "Warhammer", "0, 135, 209" },
                    { "Castellax Bronze (Metal)", "Air", "Warhammer", "130, 65, 33" },
                    { "Castellan Green", "Air", "Warhammer", "38, 71, 21" },
                    { "Chemos Purple", "Air", "Warhammer", "79, 53, 108" },
                    { "Corvus Black", "Air", "Warhammer", "23, 19, 20" },
                    { "Dawnstone", "Air", "Warhammer", "105, 112, 104" },
                    { "Death Korps Drab", "Air", "Warhammer", "61, 69, 57" },
                    { "Deathclaw Brown", "Air", "Warhammer", "175, 99, 79" },
                    { "Deathshroud (Clear)", "Air", "Warhammer", "28, 28, 27" },
                    { "Deathworld Forest", "Air", "Warhammer", "85, 98, 41" },
                    { "Dryad Bark", "Air", "Warhammer", "43, 42, 36" },
                    { "Eidolon Purple (Clear)", "Air", "Warhammer", "125, 77, 153" },
                    { "Elysian Green", "Air", "Warhammer", "107, 140, 55" },
                    { "Evil Sunz Scarlet", "Air", "Warhammer", "192, 20, 17" },
                    { "Fenrisian Grey", "Air", "Warhammer", "109, 148, 179" },
                    { "Flash Gitz Yellow", "Air", "Warhammer", "255, 243, 0" },
                    { "Gal Vorbak Red", "Air", "Warhammer", "75, 33, 60" },
                    { "Genestealer Purple", "Air", "Warhammer", "118, 88, 165" },
                    { "Grey Knights Steel (Metal)", "Air", "Warhammer", "123, 139, 150" },
                    { "Iron Hands Steel (Metal)", "Air", "Warhammer", "125, 117, 111" },
                    { "Ironbreaker (Metal)", "Air", "Warhammer", "97, 102, 103" },
                    { "Kakophoni Purple", "Air", "Warhammer", "136, 105, 174" },
                    { "Kantor Blue", "Air", "Warhammer", "2, 19, 78" },
                    { "Karak Stone", "Air", "Warhammer", "183, 148, 92" },
                    { "Khorne Red", "Air", "Warhammer", "101, 0, 1" },
                    { "Kislev Flesh", "Air", "Warhammer", "209, 165, 112" },
                    { "Leadbelcher (Metal)", "Air", "Warhammer", "87, 91, 93" },
                    { "Lothern Blue", "Air", "Warhammer", "44, 155, 204" },
                    { "Lupercal Green", "Air", "Warhammer", "0, 44, 43" },
                    { "Macragge Blue", "Air", "Warhammer", "15, 61, 124" },
                    { "Mechanicus Standard Grey", "Air", "Warhammer", "57, 72, 74" },
                    { "Mephiston Red", "Air", "Warhammer", "150, 12, 9" },
                    { "Moot Green", "Air", "Warhammer", "61, 175, 68" },
                    { "Mortarion Green (Clear)", "Air", "Warhammer", "0, 131, 43" },
                    { "Mournfang Brown", "Air", "Warhammer", "73, 15, 6" },
                    { "Night Lords Blue", "Air", "Warhammer", "0, 43, 92" },
                    { "Nocturne Green", "Air", "Warhammer", "22, 42, 41" },
                    { "Ogryn Camo", "Air", "Warhammer", "150, 166, 72" },
                    { "Phalanx Yellow", "Air", "Warhammer", "255, 226, 0" },
                    { "Phoenician Purple", "Air", "Warhammer", "68, 0, 82" },
                    { "Pyroclast Orange (Clear)", "Air", "Warhammer", "237, 128, 34" },
                    { "Relictor Gold (Metal)", "Air", "Warhammer", "193, 154, 92" },
                    { "Runefang Steel (Metal)", "Air", "Warhammer", "158, 165, 169" },
                    { "Russ Grey", "Air", "Warhammer", "80, 112, 133" },
                    { "Sigismund Yellow (Clear)", "Air", "Warhammer", "255, 227, 47" },
                    { "Sons of Horus Green", "Air", "Warhammer", "0, 84, 94" },
                    { "Steel Legion Drab", "Air", "Warhammer", "88, 78, 45" },
                    { "Straken Green", "Air", "Warhammer", "89, 127, 28" },
                    { "Sybarite Green", "Air", "Warhammer", "23, 161, 102" },
                    { "Tallarn Sand", "Air", "Warhammer", "160, 116, 9" },
                    { "Tau Light Ochre", "Air", "Warhammer", "188, 107, 16" },
                    { "Temple Guard Blue", "Air", "Warhammer", "35, 148, 137" },
                    { "Terminatus Stone", "Air", "Warhammer", "200, 183, 157" },
                    { "Thallax Gold (Metal)", "Air", "Warhammer", "182, 94, 40" },
                    { "The Fang", "Air", "Warhammer", "64, 91, 113" },
                    { "Troll Slayer Orange", "Air", "Warhammer", "241, 108, 35" },
                    { "Tuskgor Fur", "Air", "Warhammer", "134, 50, 49" },
                    { "Typhon Ash", "Air", "Warhammer", "228, 216, 193" },
                    { "Ulthuan Grey", "Air", "Warhammer", "196, 221, 213" },
                    { "Ushabti Bone", "Air", "Warhammer", "171, 161, 115" },
                    { "Valdor Gold (Metal)", "Air", "Warhammer", "160, 89, 42" },
                    { "Vulkan Green", "Air", "Warhammer", "34, 60, 46" },
                    { "Warboss Green", "Air", "Warhammer", "49, 126, 87" },
                    { "Word Bearers Red", "Air", "Warhammer", "98, 1, 4" },
                    { "White Scar", "Air", "Warhammer", "255, 255, 255" },
                    { "XV-88", "Air", "Warhammer", "108, 72, 17" },
                    { "Zandri Dust", "Air", "Warhammer", "152, 142, 86" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
