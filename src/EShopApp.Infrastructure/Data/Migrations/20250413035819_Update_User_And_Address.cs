using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_User_And_Address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "DomainUsers");

            migrationBuilder.RenameColumn(
                name: "Address_City",
                table: "DomainUsers",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "Address_Country",
                table: "DomainUsers",
                newName: "AddressLine2");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "DomainUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "DomainUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DomainUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "DomainUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "DomainUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "DomainUsers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "DomainUsers");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "DomainUsers",
                newName: "Address_City");

            migrationBuilder.RenameColumn(
                name: "AddressLine2",
                table: "DomainUsers",
                newName: "Address_Country");

            migrationBuilder.AlterColumn<string>(
                name: "Address_City",
                table: "DomainUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "DomainUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
