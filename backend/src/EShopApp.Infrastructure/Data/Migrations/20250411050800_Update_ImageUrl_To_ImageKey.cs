using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ImageUrl_To_ImageKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductImage",
                newName: "ImageKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageKey",
                table: "ProductImage",
                newName: "ImageUrl");
        }
    }
}
