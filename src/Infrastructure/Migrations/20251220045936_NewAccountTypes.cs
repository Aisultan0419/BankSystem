using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewAccountTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_ClientId",
                table: "Accounts");

            migrationBuilder.AddColumn<decimal>(
                name: "AccruedInterest",
                table: "Accounts",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentDepositClientId",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Accounts",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LockedDepositClientId",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SavingAccount_AccruedInterest",
                table: "Accounts",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SavingDepositClientId",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrentDepositClientId",
                table: "Accounts",
                column: "CurrentDepositClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LockedDepositClientId",
                table: "Accounts",
                column: "LockedDepositClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_SavingDepositClientId",
                table: "Accounts",
                column: "SavingDepositClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_ClientId",
                table: "Accounts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_CurrentDepositClientId",
                table: "Accounts",
                column: "CurrentDepositClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_LockedDepositClientId",
                table: "Accounts",
                column: "LockedDepositClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_SavingDepositClientId",
                table: "Accounts",
                column: "SavingDepositClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_ClientId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_CurrentDepositClientId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_LockedDepositClientId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_SavingDepositClientId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CurrentDepositClientId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_LockedDepositClientId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_SavingDepositClientId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AccruedInterest",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CurrentDepositClientId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "LockedDepositClientId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "SavingAccount_AccruedInterest",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "SavingDepositClientId",
                table: "Accounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_ClientId",
                table: "Accounts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
