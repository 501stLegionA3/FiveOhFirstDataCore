using Microsoft.EntityFrameworkCore.Migrations;

namespace FiveOhFirstDataCore.Data.Migrations
{
    public partial class DevSoyvolonDeletePolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyClaimData_DynamicPolicies_DynamicPolicyPolicyName",
                table: "PolicyClaimData");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyClaimData_DynamicPolicies_DynamicPolicyPolicyName",
                table: "PolicyClaimData",
                column: "DynamicPolicyPolicyName",
                principalTable: "DynamicPolicies",
                principalColumn: "PolicyName",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyClaimData_DynamicPolicies_DynamicPolicyPolicyName",
                table: "PolicyClaimData");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyClaimData_DynamicPolicies_DynamicPolicyPolicyName",
                table: "PolicyClaimData",
                column: "DynamicPolicyPolicyName",
                principalTable: "DynamicPolicies",
                principalColumn: "PolicyName",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
