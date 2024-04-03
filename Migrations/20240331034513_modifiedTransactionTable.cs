using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CBA.Migrations
{
    /// <inheritdoc />
    public partial class modifiedTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Transaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MoneyIn",
                table: "Transaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MoneyOut",
                table: "Transaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "MoneyIn",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "MoneyOut",
                table: "Transaction");
        }
    }
}
