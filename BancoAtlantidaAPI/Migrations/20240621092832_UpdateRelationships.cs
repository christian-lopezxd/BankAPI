using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoAtlantidaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_CreditCards_CreditCardId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CreditCardId1",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreditCardId1",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditCardId1",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreditCardId1",
                table: "Transactions",
                column: "CreditCardId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_CreditCards_CreditCardId1",
                table: "Transactions",
                column: "CreditCardId1",
                principalTable: "CreditCards",
                principalColumn: "Id");
        }
    }
}
