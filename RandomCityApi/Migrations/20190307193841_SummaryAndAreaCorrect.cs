using Microsoft.EntityFrameworkCore.Migrations;

namespace RandomCityApi.Migrations
{
    public partial class SummaryAndAreaCorrect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Area",
                table: "Cities",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Area",
                table: "Cities",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
