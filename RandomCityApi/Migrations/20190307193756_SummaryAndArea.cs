using Microsoft.EntityFrameworkCore.Migrations;

namespace RandomCityApi.Migrations
{
    public partial class SummaryAndArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Area",
                table: "Cities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Cities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Cities");
        }
    }
}
