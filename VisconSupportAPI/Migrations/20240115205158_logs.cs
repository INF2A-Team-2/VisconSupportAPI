using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisconSupportAPI.Migrations
{
    /// <inheritdoc />
    public partial class logs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Logs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Logs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Logs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Logs");
        }
    }
}
