using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzymandiasInvestments.Migrations.InvestmentDb
{
    public partial class Symbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Investment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Investment");
        }
    }
}
