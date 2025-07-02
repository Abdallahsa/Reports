using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reports.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusFileinReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reports");
        }
    }
}
