using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneInForgotPassWordRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForgotPasswordRequest",
                table: "ForgotPasswordRequest");

            migrationBuilder.RenameTable(
                name: "ForgotPasswordRequest",
                newName: "ForgotPasswordRequests");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ForgotPasswordRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForgotPasswordRequests",
                table: "ForgotPasswordRequests",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForgotPasswordRequests",
                table: "ForgotPasswordRequests");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ForgotPasswordRequests");

            migrationBuilder.RenameTable(
                name: "ForgotPasswordRequests",
                newName: "ForgotPasswordRequest");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForgotPasswordRequest",
                table: "ForgotPasswordRequest",
                column: "Id");
        }
    }
}
