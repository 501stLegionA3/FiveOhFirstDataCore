using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportedById = table.Column<int>(type: "integer", nullable: false),
                    ReportViewableAt = table.Column<int>(type: "integer", nullable: false),
                    ElevatedToBattalion = table.Column<bool>(type: "boolean", nullable: false),
                    Public = table.Column<bool>(type: "boolean", nullable: false),
                    Responses = table.Column<List<string>>(type: "text[]", nullable: false),
                    Resolved = table.Column<bool>(type: "boolean", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportNotificationTrackers",
                columns: table => new
                {
                    Key = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationForId = table.Column<int>(type: "integer", nullable: false),
                    LastView = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportNotificationTrackers", x => x.Key);
                    table.ForeignKey(
                        name: "FK_ReportNotificationTrackers_AspNetUsers_NotificationForId",
                        column: x => x.NotificationForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportNotificationTrackers_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportNotificationTrackers_NotificationForId",
                table: "ReportNotificationTrackers",
                column: "NotificationForId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportNotificationTrackers_ReportId",
                table: "ReportNotificationTrackers",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedById",
                table: "Reports",
                column: "ReportedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportNotificationTrackers");

            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
