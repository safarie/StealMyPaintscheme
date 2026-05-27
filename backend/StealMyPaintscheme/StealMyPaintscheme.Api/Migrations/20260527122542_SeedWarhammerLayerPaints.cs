using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerLayerPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Administratum Grey", "Layer", "Warhammer", "152, 156, 148" },
                    { "Ahriman Blue", "Layer", "Warhammer", "0, 112, 138" },
                    { "Alaitoc Blue", "Layer", "Warhammer", "47, 79, 133" },
                    { "Altdorf Guard Blue", "Layer", "Warhammer", "45, 70, 150" },
                    { "Auric Armour Gold (Metal)", "Layer", "Warhammer", "218, 143, 52" },
                    { "Baharroth Blue", "Layer", "Warhammer", "84, 189, 202" },
                    { "Baneblade Brown", "Layer", "Warhammer", "143, 124, 104" },
                    { "Balor Brown", "Layer", "Warhammer", "135, 84, 8" },
                    { "Bestigor Flesh", "Layer", "Warhammer", "208, 137, 81" },
                    { "Bloodreaver Flesh", "Layer", "Warhammer", "106, 72, 72" },
                    { "Blue Horror", "Layer", "Warhammer", "158, 181, 206" },
                    { "Brass Scorpion (Metal)", "Layer", "Warhammer", "111, 45, 19" },
                    { "Cadian Fleshtone", "Layer", "Warhammer", "196, 118, 82" },
                    { "Calgar Blue", "Layer", "Warhammer", "42, 73, 127" },
                    { "Castellax Bronze (Metal)", "Layer", "Warhammer", "127, 62, 32" },
                    { "Canoptek Alloy", "Layer", "Warhammer", "168, 144, 138" },
                    { "Dark Reaper", "Layer", "Warhammer", "53, 77, 76" },
                    { "Dawnstone", "Layer", "Warhammer", "105, 112, 104" },
                    { "Deathclaw Brown", "Layer", "Warhammer", "175, 99, 79" },
                    { "Dechala Lilac", "Layer", "Warhammer", "181, 152, 201" },
                    { "Deepkin Flesh", "Layer", "Warhammer", "169, 183, 159" },
                    { "Doombull Brown", "Layer", "Warhammer", "87, 0, 3" },
                    { "Dorn Yellow", "Layer", "Warhammer", "255, 245, 90" },
                    { "Elysian Green", "Layer", "Warhammer", "107, 140, 55" },
                    { "Emperor's Children", "Layer", "Warhammer", "183, 64, 115" },
                    { "Eshin Grey", "Layer", "Warhammer", "72, 75, 78" },
                    { "Evil Sunz Scarlet", "Layer", "Warhammer", "192, 20, 17" },
                    { "Fenrisian Grey", "Layer", "Warhammer", "109, 148, 179" },
                    { "Fire Dragon Bright", "Layer", "Warhammer", "244, 135, 78" },
                    { "Flash Gitz Yellow", "Layer", "Warhammer", "255, 243, 0" },
                    { "Flayed One Flesh", "Layer", "Warhammer", "238, 196, 131" },
                    { "Fulgrim Pink", "Layer", "Warhammer", "243, 171, 202" },
                    { "Fulgurite Copper (Metal)", "Layer", "Warhammer", "158, 87, 41" },
                    { "Gauss Blaster Green", "Layer", "Warhammer", "127, 193, 165" },
                    { "Gehenna's Gold (Metal)", "Layer", "Warhammer", "159, 71, 16" },
                    { "Genestealer Purple", "Layer", "Warhammer", "118, 88, 165" },
                    { "Gorthor Brown", "Layer", "Warhammer", "95, 70, 63" },
                    { "Hashut Copper (Metal)", "Layer", "Warhammer", "136, 89, 57" },
                    { "Hoeth Blue", "Layer", "Warhammer", "76, 120, 175" },
                    { "Ironbreaker (Metal)", "Layer", "Warhammer", "97, 102, 103" },
                    { "Kabalite Green", "Layer", "Warhammer", "0, 137, 98" },
                    { "Kakophoni Purple", "Layer", "Warhammer", "136, 105, 174" },
                    { "Karak Stone", "Layer", "Warhammer", "183, 148, 92" },
                    { "Kislev Flesh", "Layer", "Warhammer", "209, 165, 112" },
                    { "Knight-Questor Flesh", "Layer", "Warhammer", "153, 101, 99" },
                    { "Krieg Khaki", "Layer", "Warhammer", "188, 187, 126" },
                    { "Liberator Gold (Metal)", "Layer", "Warhammer", "186, 145, 79" },
                    { "Loren Forest", "Layer", "Warhammer", "72, 108, 37" },
                    { "Lothern Blue", "Layer", "Warhammer", "44, 155, 204" },
                    { "Lugganath Orange", "Layer", "Warhammer", "246, 155, 130" },
                    { "Moot Green", "Layer", "Warhammer", "61, 175, 68" },
                    { "Nurgling Green", "Layer", "Warhammer", "126, 151, 94" },
                    { "Ogryn Camo", "Layer", "Warhammer", "150, 166, 72" },
                    { "Pallid Wych Flesh", "Layer", "Warhammer", "202, 204, 187" },
                    { "Phalanx Yellow", "Layer", "Warhammer", "255, 226, 0" },
                    { "Pink Horror", "Layer", "Warhammer", "142, 39, 87" },
                    { "Runefang Steel (Metal)", "Layer", "Warhammer", "158, 165, 169" },
                    { "Runelord Brass (Metal)", "Layer", "Warhammer", "87, 71, 54" },
                    { "Russ Grey", "Layer", "Warhammer", "80, 112, 133" },
                    { "Screaming Skull", "Layer", "Warhammer", "185, 192, 153" },
                    { "Skarsnik Green", "Layer", "Warhammer", "88, 143, 107" },
                    { "Skavenblight Dinge", "Layer", "Warhammer", "69, 65, 59" },
                    { "Skrag Brown", "Layer", "Warhammer", "139, 72, 6" },
                    { "Skullcrusher Brass (Metal)", "Layer", "Warhammer", "222, 170, 87" },
                    { "Slaanesh Grey", "Layer", "Warhammer", "139, 136, 147" },
                    { "Sons of Horus Green", "Layer", "Warhammer", "0, 84, 94" },
                    { "Sotek Green", "Layer", "Warhammer", "11, 99, 113" },
                    { "Squig Orange", "Layer", "Warhammer", "167, 77, 66" },
                    { "Stormhost Silver (Metal)", "Layer", "Warhammer", "191, 196, 198" },
                    { "Stormvermin Fur", "Layer", "Warhammer", "109, 101, 95" },
                    { "Straken Green", "Layer", "Warhammer", "89, 127, 28" },
                    { "Sybarite Green", "Layer", "Warhammer", "23, 161, 102" },
                    { "Sycorax Bronze (Metal)", "Layer", "Warhammer", "147, 112, 90" },
                    { "Tallarn Sand", "Layer", "Warhammer", "160, 116, 9" },
                    { "Tau Light Ochre", "Layer", "Warhammer", "188, 107, 16" },
                    { "Teclis Blue", "Layer", "Warhammer", "56, 119, 191" },
                    { "Temple Guard Blue", "Layer", "Warhammer", "35, 148, 137" },
                    { "Thunderhawk Blue", "Layer", "Warhammer", "57, 106, 112" },
                    { "Troll Slayer Orange", "Layer", "Warhammer", "241, 108, 35" },
                    { "Tuskgor Fur", "Layer", "Warhammer", "134, 50, 49" },
                    { "Ulthuan Grey", "Layer", "Warhammer", "196, 221, 213" },
                    { "Ungor Flesh", "Layer", "Warhammer", "209, 165, 96" },
                    { "Ushabti Bone", "Layer", "Warhammer", "171, 161, 115" },
                    { "Vulkan Green", "Layer", "Warhammer", "34, 60, 46" },
                    { "Warboss Green", "Layer", "Warhammer", "49, 126, 87" },
                    { "Warpfiend Grey", "Layer", "Warhammer", "102, 101, 110" },
                    { "Word Bearers Red", "Layer", "Warhammer", "98, 1, 4" },
                    { "Wild Rider Red", "Layer", "Warhammer", "232, 46, 27" },
                    { "White Scar", "Layer", "Warhammer", "255, 255, 255" },
                    { "Wazdakka Red", "Layer", "Warhammer", "136, 8, 4" },
                    { "Warpstone Glow", "Layer", "Warhammer", "15, 112, 42" },
                    { "Xereus Purple", "Layer", "Warhammer", "71, 18, 90" },
                    { "Yriel Yellow", "Layer", "Warhammer", "255, 217, 0" },
                    { "Zamesi Desert", "Layer", "Warhammer", "216, 157, 27" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
