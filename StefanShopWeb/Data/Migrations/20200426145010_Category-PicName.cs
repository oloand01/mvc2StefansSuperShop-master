using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StefanShopWeb.Data.Migrations
{
    public partial class CategoryPicName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureName",
                table: "Categories",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "Categories");
        }
    }
}
