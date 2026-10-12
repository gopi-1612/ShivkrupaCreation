using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShivkrupaCreation.API.Migrations
{
    /// <inheritdoc />
    public partial class updateProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BottomStyle",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Neck",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Occasion",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SetType",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SleeveStyle",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StitchType",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Work",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BottomStyle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Neck",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Occasion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SetType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SleeveStyle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StitchType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Style",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Work",
                table: "Products");
        }
    }
}
