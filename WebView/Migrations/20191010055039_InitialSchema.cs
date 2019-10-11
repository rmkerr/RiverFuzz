using Microsoft.EntityFrameworkCore.Migrations;

namespace WebView.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnownEndpoints",
                columns: table => new
                {
                    EndpointModelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownEndpoints", x => x.EndpointModelID);
                });

            migrationBuilder.CreateTable(
                name: "KnownRequests",
                columns: table => new
                {
                    RequestModelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndpointModelID = table.Column<int>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    RequestText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownRequests", x => x.RequestModelID);
                    table.ForeignKey(
                        name: "FK_KnownRequests_KnownEndpoints_EndpointModelID",
                        column: x => x.EndpointModelID,
                        principalTable: "KnownEndpoints",
                        principalColumn: "EndpointModelID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnownRequests_EndpointModelID",
                table: "KnownRequests",
                column: "EndpointModelID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnownRequests");

            migrationBuilder.DropTable(
                name: "KnownEndpoints");
        }
    }
}
