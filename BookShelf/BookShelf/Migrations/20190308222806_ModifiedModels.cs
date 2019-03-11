using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CS1591

namespace BookShelf.Migrations
{
    public partial class ModifiedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Books",
                newName: "ImagePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Books",
                newName: "ImageUrl");
        }
    }
}
