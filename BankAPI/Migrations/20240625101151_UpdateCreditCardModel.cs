using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoAtlantidaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreditCardModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardHolder",
                table: "CreditCards",
                newName: "LastName");

            migrationBuilder.AddColumn<decimal>(
                name: "BonusInterest",
                table: "CreditCards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPaymentDue",
                table: "CreditCards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountWithInterest",
                table: "CreditCards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusInterest",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "MinimumPaymentDue",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "TotalAmountWithInterest",
                table: "CreditCards");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "CreditCards",
                newName: "CardHolder");
        }
    }
}
