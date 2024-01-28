using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBA.Migrations
{
    /// <inheritdoc />
    public partial class testing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankBranch_AspNetUsers_UserId",
                table: "BankBranch");

            migrationBuilder.DropIndex(
                name: "IX_BankBranch_UserId",
                table: "BankBranch");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BankBranch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BranchUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchUser_BankBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BankBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLAccounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchUser_BranchId",
                table: "BranchUser",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchUser_UserId",
                table: "BranchUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchUser");

            migrationBuilder.DropTable(
                name: "GLAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BankBranch",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
