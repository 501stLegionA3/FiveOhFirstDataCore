using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonMPFieldAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MilitaryPolice",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MilitaryPolice",
                table: "AspNetUsers");
        }
    }
}
