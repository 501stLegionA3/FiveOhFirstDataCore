using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Data.Migrations
{
    public partial class ReleaseSoyvolonVersionZeroSixteenZero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostActions",
                columns: table => new
                {
                    Action = table.Column<int>(type: "integer", nullable: false),
                    DiscordChannel = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RawMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostActions", x => x.Action);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostActions");
        }
    }
}
