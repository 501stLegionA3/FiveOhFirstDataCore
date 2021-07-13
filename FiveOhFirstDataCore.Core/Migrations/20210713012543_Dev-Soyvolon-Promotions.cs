using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonPromotions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionForId = table.Column<int>(type: "integer", nullable: false),
                    RequestedById = table.Column<int>(type: "integer", nullable: true),
                    NeededBoard = table.Column<int>(type: "integer", nullable: false),
                    CurrentBoard = table.Column<int>(type: "integer", nullable: false),
                    PromoteFrom = table.Column<int>(type: "integer", nullable: false),
                    PromoteTo = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_AspNetUsers_PromotionForId",
                        column: x => x.PromotionForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promotions_AspNetUsers_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PromotionTrooper",
                columns: table => new
                {
                    ApprovedById = table.Column<int>(type: "integer", nullable: false),
                    ApprovedPromotionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionTrooper", x => new { x.ApprovedById, x.ApprovedPromotionsId });
                    table.ForeignKey(
                        name: "FK_PromotionTrooper_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionTrooper_Promotions_ApprovedPromotionsId",
                        column: x => x.ApprovedPromotionsId,
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_PromotionForId",
                table: "Promotions",
                column: "PromotionForId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_RequestedById",
                table: "Promotions",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionTrooper_ApprovedPromotionsId",
                table: "PromotionTrooper",
                column: "ApprovedPromotionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionTrooper");

            migrationBuilder.DropTable(
                name: "Promotions");
        }
    }
}
