using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class inventory_transaction_fk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "InventoryTransactions",
                newName: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_InventoryId",
                table: "InventoryTransactions",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Inventories_InventoryId",
                table: "InventoryTransactions",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Inventories_InventoryId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_InventoryId",
                table: "InventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "InventoryTransactions",
                newName: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_ProductId",
                table: "InventoryTransactions",
                column: "ProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
