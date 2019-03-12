using Microsoft.EntityFrameworkCore.Migrations;

namespace RandomCityApi.Migrations
{
    public partial class WikiReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WikiPop",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WikiRef",
                table: "Cities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WikiPop",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "WikiRef",
                table: "Cities");
        }
    }
}
