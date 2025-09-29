using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPincode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountOfPinAttempts",
                table: "Cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "Cards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedUntil",
                table: "AppUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountOfLoginAttempts",
                table: "AppUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HashedPinCode",
                table: "AppUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountOfPinAttempts",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "BlockedUntil",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "CountOfLoginAttempts",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "HashedPinCode",
                table: "AppUsers");
        }
    }
}
