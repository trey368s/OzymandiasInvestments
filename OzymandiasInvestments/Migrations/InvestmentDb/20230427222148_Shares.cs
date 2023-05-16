using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzymandiasInvestments.Migrations.InvestmentDb
{
    public partial class Shares : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Investment",
                newName: "Shares");

            migrationBuilder.AddColumn<decimal>(
                name: "OpenPrice",
                table: "Investment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenPrice",
                table: "Investment");

            migrationBuilder.RenameColumn(
                name: "Shares",
                table: "Investment",
                newName: "Price");
        }
    }
}
