using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Data.Migrations
{
    public partial class DevHobNCORanks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NCORank",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NCORank",
                table: "AspNetUsers");

        }
    }
}
