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

            migrationBuilder.CreateTable(
                name: "AdminNewsletterViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminNewsletterViewModel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminNewsletterViewModel");

            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "Categories");
        }
    }
}
