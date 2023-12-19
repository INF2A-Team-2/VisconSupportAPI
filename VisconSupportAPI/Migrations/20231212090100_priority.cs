using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisconSupportAPI.Migrations
{
    /// <inheritdoc />
    public partial class priority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Issues",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Issues");
        }
    }
}
