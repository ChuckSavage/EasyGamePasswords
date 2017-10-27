using Microsoft.EntityFrameworkCore.Migrations;

namespace ToClipboard.Migrations
{
    public partial class SortBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortBy",
                table: "JumpLists",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortBy",
                table: "JumpLists");
        }
    }
}
