using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Data.Migrations
{
    public partial class ReleaseSoyvolonVersionZeroFifteenZero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PFP",
                table: "AspNetUsers",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PFP",
                table: "AspNetUsers");
        }
    }
}
