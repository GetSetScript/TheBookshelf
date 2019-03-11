﻿using Microsoft.EntityFrameworkCore.Migrations;

#pragma warning disable CS1591

namespace BookShelf.Migrations
{
    public partial class RevisedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Books");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Books",
                nullable: false,
                defaultValue: "");
        }
    }
}
