using Microsoft.EntityFrameworkCore.Migrations;

namespace GatefailBot.Migrations
{
    public partial class moduleconfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ModuleName = table.Column<string>(nullable: true),
                    Activated = table.Column<bool>(nullable: false),
                    GuildId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleConfigurations_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_GuildId",
                table: "ModuleConfigurations",
                column: "GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleConfigurations");
        }
    }
}
