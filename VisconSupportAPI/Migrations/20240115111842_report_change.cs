using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisconSupportAPI.Migrations
{
    /// <inheritdoc />
    public partial class report_change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Attachments_AttachmentId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Issues_IssueId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Machines_MachineId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Messages_MessageId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Companies_CompanyId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Issues_IssueId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_IssueId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Logs_AttachmentId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_IssueId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_MachineId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_MessageId",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_UserId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Companies_CompanyId",
                table: "Reports",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Companies_CompanyId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "IssueId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IssueId",
                table: "Reports",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_AttachmentId",
                table: "Logs",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IssueId",
                table: "Logs",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_MachineId",
                table: "Logs",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_MessageId",
                table: "Logs",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Attachments_AttachmentId",
                table: "Logs",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Issues_IssueId",
                table: "Logs",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Machines_MachineId",
                table: "Logs",
                column: "MachineId",
                principalTable: "Machines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Messages_MessageId",
                table: "Logs",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Companies_CompanyId",
                table: "Reports",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Issues_IssueId",
                table: "Reports",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
