using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonepar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeForId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    RTORank = table.Column<int>(type: "integer", nullable: true),
                    MedicRank = table.Column<int>(type: "integer", nullable: true),
                    WarrantRank = table.Column<int>(type: "integer", nullable: true),
                    WardenRank = table.Column<int>(type: "integer", nullable: true),
                    PilotRank = table.Column<int>(type: "integer", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: true),
                    Team = table.Column<int>(type: "integer", nullable: true),
                    Flight = table.Column<int>(type: "integer", nullable: true),
                    Slot = table.Column<int>(type: "integer", nullable: true),
                    Qualifications = table.Column<long>(type: "bigint", nullable: false),
                    LastPromotion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StartOfService = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastBilletChange = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GraduatedBCTOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GraduatedUTCOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AdditionalChanges = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.Key);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_AspNetUsers_ChangeForId",
                        column: x => x.ChangeForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ChangeForId",
                table: "ChangeRequests",
                column: "ChangeForId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeRequests");
        }
    }
}
