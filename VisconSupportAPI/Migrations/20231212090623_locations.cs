using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisconSupportAPI.Migrations
{
    /// <inheritdoc />
    public partial class locations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Companies",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Companies",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Companies");
        }
    }
}
