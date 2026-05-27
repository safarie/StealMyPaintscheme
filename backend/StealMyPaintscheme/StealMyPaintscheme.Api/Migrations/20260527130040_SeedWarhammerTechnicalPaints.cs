using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealMyPaintscheme.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedWarhammerTechnicalPaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GlobalPaints",
                columns: new[] { "Name", "Type", "Maker", "RGB" },
                values: new object[,]
                {
                    { "Agrellan Earth", "Technical", "Warhammer", "179, 158, 128" },
                    { "Agrellan Badland", "Technical", "Warhammer", "179, 158, 128" },
                    { "Ardcoat", "Technical", "Warhammer", "255, 255, 255" },
                    { "Armageddon Dunes", "Technical", "Warhammer", "232, 211, 111" },
                    { "Armageddon Dust", "Technical", "Warhammer", "232, 211, 111" },
                    { "Astrogranite Debris", "Technical", "Warhammer", "157, 157, 157" },
                    { "Astrogranite", "Technical", "Warhammer", "157, 157, 157" },
                    { "Blood for the Blood God", "Technical", "Warhammer", "158, 99, 102" },
                    { "Contrast Medium", "Technical", "Warhammer", "255, 255, 255" },
                    { "Hexwraith Flame", "Technical", "Warhammer", "0, 162, 55" },
                    { "Lahmian Medium", "Technical", "Warhammer", "255, 255, 255" },
                    { "Martian Ironcrust", "Technical", "Warhammer", "207, 112, 93" },
                    { "Martian Ironearth", "Technical", "Warhammer", "207, 112, 93" },
                    { "Mordant Earth", "Technical", "Warhammer", "23, 19, 20" },
                    { "Nighthaunt Gloom", "Technical", "Warhammer", "76, 131, 138" },
                    { "Nihilakh Oxide", "Technical", "Warhammer", "102, 179, 154" },
                    { "Nurgles Rot", "Technical", "Warhammer", "157, 139, 22" },
                    { "Soulstone Blue", "Technical", "Warhammer", "98, 110, 143" },
                    { "Spiritstone Red", "Technical", "Warhammer", "174, 110, 100" },
                    { "Stirland Battlemire", "Technical", "Warhammer", "112, 73, 13" },
                    { "Stirland Mud", "Technical", "Warhammer", "112, 73, 13" },
                    { "Storm Shield", "Technical", "Warhammer", "255, 255, 255" },
                    { "Tesseract Glow", "Technical", "Warhammer", "73, 173, 51" },
                    { "Typhus Corrosion", "Technical", "Warhammer", "55, 58, 34" },
                    { "Valhallan Blizzard", "Technical", "Warhammer", "225, 225, 225" },
                    { "Waystone Green", "Technical", "Warhammer", "102, 136, 120" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
