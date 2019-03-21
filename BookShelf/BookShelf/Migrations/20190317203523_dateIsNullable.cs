using System;
using Microsoft.EntityFrameworkCore.Migrations;
#pragma warning disable CS1591
namespace BookShelf.Migrations
{
    public partial class dateIsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                maxLength: 55,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRead",
                table: "Books",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                maxLength: 55,
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 55);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRead",
                table: "Books",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 55);
        }
    }
}
