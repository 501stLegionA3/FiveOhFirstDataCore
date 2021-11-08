using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Data.Migrations
{
    public partial class ReleaseSoyvolonVersionZeroEighteenOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QualsChanged",
                table: "ChangeRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QualsChanged",
                table: "ChangeRequests");
        }
    }
}
