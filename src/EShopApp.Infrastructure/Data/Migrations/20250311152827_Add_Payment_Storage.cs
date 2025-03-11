using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Payment_Storage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
