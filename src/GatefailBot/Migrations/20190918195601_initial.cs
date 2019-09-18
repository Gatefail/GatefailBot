using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatefailBot.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchQueries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UniqueId = table.Column<Guid>(nullable: false),
                    IdToQuery = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchQueries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommandChannelRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordChannelId = table.Column<ulong>(nullable: false),
                    Command = table.Column<string>(nullable: false),
                    NormalizedCommand = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandChannelRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DiscordId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DiscordId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DiscordId",
                table: "Users",
                column: "DiscordId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GuildId",
                table: "Users",
                column: "GuildId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchQueries");

            migrationBuilder.DropTable(
                name: "CommandChannelRestrictions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
