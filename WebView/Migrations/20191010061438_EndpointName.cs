using Microsoft.EntityFrameworkCore.Migrations;

namespace WebView.Migrations
{
    public partial class EndpointName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "KnownEndpoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "KnownEndpoints");
        }
    }
}
