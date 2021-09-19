using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Data.Migrations
{
    public partial class ReleaseSoyvolonVersionZeroFourZero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimUpdates_AspNetUsers_ChangedById",
                table: "ClaimUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_CShopUpdates_AspNetUsers_ChangedById",
                table: "CShopUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_NickNameUpdate_AspNetUsers_ApprovedById",
                table: "NickNameUpdate");

            migrationBuilder.DropForeignKey(
                name: "FK_Notices_AspNetUsers_AuthorId",
                table: "Notices");

            migrationBuilder.DropForeignKey(
                name: "FK_RankUpdates_AspNetUsers_ChangedById",
                table: "RankUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_RecruitmentUpdates_AspNetUsers_RecruitedById",
                table: "RecruitmentUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeUpdates_AspNetUsers_ChangedById",
                table: "TimeUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_TrooperFlags_AspNetUsers_AuthorId",
                table: "TrooperFlags");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "TrooperFlags",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "TimeUpdates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "RecruitedById",
                table: "RecruitmentUpdates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "RankUpdates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Notices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedById",
                table: "NickNameUpdate",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "CShopUpdates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "ClaimUpdates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimUpdates_AspNetUsers_ChangedById",
                table: "ClaimUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CShopUpdates_AspNetUsers_ChangedById",
                table: "CShopUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NickNameUpdate_AspNetUsers_ApprovedById",
                table: "NickNameUpdate",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_AspNetUsers_AuthorId",
                table: "Notices",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RankUpdates_AspNetUsers_ChangedById",
                table: "RankUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RecruitmentUpdates_AspNetUsers_RecruitedById",
                table: "RecruitmentUpdates",
                column: "RecruitedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeUpdates_AspNetUsers_ChangedById",
                table: "TimeUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TrooperFlags_AspNetUsers_AuthorId",
                table: "TrooperFlags",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimUpdates_AspNetUsers_ChangedById",
                table: "ClaimUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_CShopUpdates_AspNetUsers_ChangedById",
                table: "CShopUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_NickNameUpdate_AspNetUsers_ApprovedById",
                table: "NickNameUpdate");

            migrationBuilder.DropForeignKey(
                name: "FK_Notices_AspNetUsers_AuthorId",
                table: "Notices");

            migrationBuilder.DropForeignKey(
                name: "FK_RankUpdates_AspNetUsers_ChangedById",
                table: "RankUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_RecruitmentUpdates_AspNetUsers_RecruitedById",
                table: "RecruitmentUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeUpdates_AspNetUsers_ChangedById",
                table: "TimeUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_TrooperFlags_AspNetUsers_AuthorId",
                table: "TrooperFlags");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "TrooperFlags",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "TimeUpdates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RecruitedById",
                table: "RecruitmentUpdates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "RankUpdates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Notices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedById",
                table: "NickNameUpdate",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "CShopUpdates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChangedById",
                table: "ClaimUpdates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimUpdates_AspNetUsers_ChangedById",
                table: "ClaimUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CShopUpdates_AspNetUsers_ChangedById",
                table: "CShopUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NickNameUpdate_AspNetUsers_ApprovedById",
                table: "NickNameUpdate",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_AspNetUsers_AuthorId",
                table: "Notices",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RankUpdates_AspNetUsers_ChangedById",
                table: "RankUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecruitmentUpdates_AspNetUsers_RecruitedById",
                table: "RecruitmentUpdates",
                column: "RecruitedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeUpdates_AspNetUsers_ChangedById",
                table: "TimeUpdates",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrooperFlags_AspNetUsers_AuthorId",
                table: "TrooperFlags",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
