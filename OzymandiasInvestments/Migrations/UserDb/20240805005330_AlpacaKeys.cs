using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzymandiasInvestments.Migrations
{
    public partial class AlpacaKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlpacaApiKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AlpacaApiSecret",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlpacaApiKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AlpacaApiSecret",
                table: "AspNetUsers");
        }
    }
}
