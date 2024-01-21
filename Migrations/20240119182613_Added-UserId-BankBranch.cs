using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBA.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserIdBankBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BankBranch_BankBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BankBranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BankBranchId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BankBranch",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankBranch",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankBranch_UserId",
                table: "BankBranch",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankBranch_AspNetUsers_UserId",
                table: "BankBranch",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankBranch_AspNetUsers_UserId",
                table: "BankBranch");

            migrationBuilder.DropIndex(
                name: "IX_BankBranch_UserId",
                table: "BankBranch");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BankBranch");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "BankBranchId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BankBranchId",
                table: "AspNetUsers",
                column: "BankBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BankBranch_BankBranchId",
                table: "AspNetUsers",
                column: "BankBranchId",
                principalTable: "BankBranch",
                principalColumn: "Id");
        }
    }
}
