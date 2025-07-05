using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusInApprovalReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Approval");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Approval",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Approval");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Approval",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
