using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRejecttedAndCurrentApprovalLevelInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Approval",
                newName: "Geha");

            migrationBuilder.AddColumn<int>(
                name: "CurrentApprovalLevel",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentApprovalLevel",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "Geha",
                table: "Approval",
                newName: "Role");
        }
    }
}
