using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Core.Migrations
{
    public partial class DevSoyvolonPromotions1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionTrooper_Promotions_ApprovedPromotionsId",
                table: "PromotionTrooper");

            migrationBuilder.DropForeignKey(
                name: "FK_RankUpdates_AspNetUsers_ChangedById",
                table: "RankUpdates");

            migrationBuilder.RenameColumn(
                name: "ChangedById",
                table: "RankUpdates",
                newName: "RequestedById");

            migrationBuilder.RenameIndex(
                name: "IX_RankUpdates_ChangedById",
                table: "RankUpdates",
                newName: "IX_RankUpdates_RequestedById");

            migrationBuilder.RenameColumn(
                name: "ApprovedPromotionsId",
                table: "PromotionTrooper",
                newName: "ApprovedPendingPromotionsId");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionTrooper_ApprovedPromotionsId",
                table: "PromotionTrooper",
                newName: "IX_PromotionTrooper_ApprovedPendingPromotionsId");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "RankUpdates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeniedById",
                table: "RankUpdates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RankUpdateTrooper",
                columns: table => new
                {
                    ApprovedById = table.Column<int>(type: "integer", nullable: false),
                    ApprovedRankUpdatesChangeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankUpdateTrooper", x => new { x.ApprovedById, x.ApprovedRankUpdatesChangeId });
                    table.ForeignKey(
                        name: "FK_RankUpdateTrooper_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RankUpdateTrooper_RankUpdates_ApprovedRankUpdatesChangeId",
                        column: x => x.ApprovedRankUpdatesChangeId,
                        principalTable: "RankUpdates",
                        principalColumn: "ChangeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankUpdates_DeniedById",
                table: "RankUpdates",
                column: "DeniedById");

            migrationBuilder.CreateIndex(
                name: "IX_RankUpdateTrooper_ApprovedRankUpdatesChangeId",
                table: "RankUpdateTrooper",
                column: "ApprovedRankUpdatesChangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionTrooper_Promotions_ApprovedPendingPromotionsId",
                table: "PromotionTrooper",
                column: "ApprovedPendingPromotionsId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RankUpdates_AspNetUsers_DeniedById",
                table: "RankUpdates",
                column: "DeniedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RankUpdates_AspNetUsers_RequestedById",
                table: "RankUpdates",
                column: "RequestedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionTrooper_Promotions_ApprovedPendingPromotionsId",
                table: "PromotionTrooper");

            migrationBuilder.DropForeignKey(
                name: "FK_RankUpdates_AspNetUsers_DeniedById",
                table: "RankUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_RankUpdates_AspNetUsers_RequestedById",
                table: "RankUpdates");

            migrationBuilder.DropTable(
                name: "RankUpdateTrooper");

            migrationBuilder.DropIndex(
                name: "IX_RankUpdates_DeniedById",
                table: "RankUpdates");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "RankUpdates");

            migrationBuilder.DropColumn(
                name: "DeniedById",
                table: "RankUpdates");

            migrationBuilder.RenameColumn(
                name: "RequestedById",
                table: "RankUpdates",
                newName: "ChangedById");

            migrationBuilder.RenameIndex(
                name: "IX_RankUpdates_RequestedById",
                table: "RankUpdates",
                newName: "IX_RankUpdates_ChangedById");

            migrationBuilder.RenameColumn(
                name: "ApprovedPendingPromotionsId",
                table: "PromotionTrooper",
                newName: "ApprovedPromotionsId");

            migrationBuilder.RenameIndex(
                name: "IX_PromotionTrooper_ApprovedPendingPromotionsId",
                table: "PromotionTrooper",
                newName: "IX_PromotionTrooper_ApprovedPromotionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionTrooper_Promotions_ApprovedPromotionsId",
                table: "PromotionTrooper",
                column: "ApprovedPromotionsId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RankUpdates_AspNetUsers_ChangedById",
                table: "RankUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
