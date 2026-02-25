using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedInterestHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmountAccrued",
                table: "InterestAccrualHistory",
                newName: "AmountAccruedToday");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAccruedApplied",
                table: "InterestAccrualHistory",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountAccruedApplied",
                table: "InterestAccrualHistory");

            migrationBuilder.RenameColumn(
                name: "AmountAccruedToday",
                table: "InterestAccrualHistory",
                newName: "AmountAccrued");
        }
    }
}
