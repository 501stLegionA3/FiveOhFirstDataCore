using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDataCore.Data.Migrations
{
    public partial class DevThunderNavBar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NavModules",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Href = table.Column<string>(type: "text", nullable: true),
                    HasMainPage = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavModules", x => x.Key);
                    table.ForeignKey(
                        name: "FK_NavModules_NavModules_ParentId",
                        column: x => x.ParentId,
                        principalTable: "NavModules",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NavModules_ParentId",
                table: "NavModules",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavModules");
        }
    }
}
