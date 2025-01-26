using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class product_no_soft_deletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }
    }
}
