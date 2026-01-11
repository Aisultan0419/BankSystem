using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestRateToSavingAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "_interestRate_EffectiveFrom",
                table: "Accounts",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "_interestRate_EffectiveTo",
                table: "Accounts",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "_interestRate_Rate",
                table: "Accounts",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_interestRate_Type",
                table: "Accounts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_interestRate_EffectiveFrom",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "_interestRate_EffectiveTo",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "_interestRate_Rate",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "_interestRate_Type",
                table: "Accounts");
        }
    }
}
