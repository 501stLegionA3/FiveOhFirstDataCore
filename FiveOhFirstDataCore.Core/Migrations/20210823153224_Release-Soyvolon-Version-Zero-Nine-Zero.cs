using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class ReleaseSoyvolonVersionZeroNineZero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                columns: table => new
                {
                    ChangeId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    Reason = table.Column<string>(type: "text", nullable: true),
                    FinalizedById = table.Column<int>(type: "integer", nullable: true),
                    Finalized = table.Column<bool>(type: "boolean", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ChangedForId = table.Column<int>(type: "integer", nullable: false),
                    SubmittedByRosterClerk = table.Column<bool>(type: "boolean", nullable: false),
                    RevertChange = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.ChangeId);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_AspNetUsers_ChangedForId",
                        column: x => x.ChangedForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_AspNetUsers_FinalizedById",
                        column: x => x.FinalizedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ChangedForId",
                table: "ChangeRequests",
                column: "ChangedForId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_FinalizedById",
                table: "ChangeRequests",
                column: "FinalizedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeRequests");
        }
    }
}
