using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzymandiasInvestments.Migrations.InvestmentDb
{
    public partial class close : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Investment",
                newName: "OpenTime");

            migrationBuilder.AddColumn<decimal>(
                name: "ClosePrice",
                table: "Investment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CloseTime",
                table: "Investment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Profit",
                table: "Investment",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosePrice",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "Profit",
                table: "Investment");

            migrationBuilder.RenameColumn(
                name: "OpenTime",
                table: "Investment",
                newName: "Time");
        }
    }
}
