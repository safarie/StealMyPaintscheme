using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerDryPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Astorath Red", "Dry", "Warhammer", "169, 49, 30" },
                    { "Changeling Pink", "Dry", "Warhammer", "243, 171, 202" },
                    { "Chronus Blue", "Dry", "Warhammer", "75, 144, 207" },
                    { "Dawnstone", "Dry", "Warhammer", "105, 112, 104" },
                    { "Eldar Flesh", "Dry", "Warhammer", "232, 192, 127" },
                    { "Etherium Blue", "Dry", "Warhammer", "158, 181, 206" },
                    { "Golden Griffon (Metal)", "Dry", "Warhammer", "190, 149, 82" },
                    { "Golgfag Brown", "Dry", "Warhammer", "143, 80, 42" },
                    { "Hellion Green", "Dry", "Warhammer", "127, 193, 165" },
                    { "Hexos Palesun", "Dry", "Warhammer", "255, 245, 90" },
                    { "Hoeth Blue", "Dry", "Warhammer", "76, 120, 175" },
                    { "Imrik Blue", "Dry", "Warhammer", "32, 138, 191" },
                    { "Kindleflame", "Dry", "Warhammer", "246, 156, 130" },
                    { "Longbeard Grey", "Dry", "Warhammer", "219, 220, 198" },
                    { "Lucius Lilac", "Dry", "Warhammer", "181, 152, 201" },
                    { "Necron Compound (Metal)", "Dry", "Warhammer", "193, 197, 200" },
                    { "Niblet Green", "Dry", "Warhammer", "55, 140, 53" },
                    { "Nurgling Green", "Dry", "Warhammer", "126, 151, 94" },
                    { "Praxeti White", "Dry", "Warhammer", "255, 255, 255" },
                    { "Ryza Rust", "Dry", "Warhammer", "241, 108, 35" },
                    { "Sigmarite (Metal)", "Dry", "Warhammer", "204, 152, 71" },
                    { "Skink Blue", "Dry", "Warhammer", "84, 189, 202" },
                    { "Slaanesh Grey", "Dry", "Warhammer", "139, 136, 147" },
                    { "Stormfang", "Dry", "Warhammer", "90, 127, 163" },
                    { "Sylvaneth Bark", "Dry", "Warhammer", "78, 72, 59" },
                    { "Terminatus Stone", "Dry", "Warhammer", "200, 183, 157" },
                    { "Thunderhawk Blue", "Dry", "Warhammer", "57, 106, 112" },
                    { "Tyrant Skull", "Dry", "Warhammer", "200, 196, 131" },
                    { "Underhive Ash", "Dry", "Warhammer", "188, 187, 126" },
                    { "Verminlord Hide", "Dry", "Warhammer", "126, 51, 49" },
                    { "Wrack White", "Dry", "Warhammer", "211, 208, 207" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
