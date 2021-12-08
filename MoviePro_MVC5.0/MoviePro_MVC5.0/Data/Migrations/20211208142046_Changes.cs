using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviePro_MVC5._0.Data.Migrations
{
    public partial class Changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MovieCollection",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RuntTime",
                table: "Movie",
                newName: "RunTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "MovieCollection",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RunTime",
                table: "Movie",
                newName: "RuntTime");
        }
    }
}
