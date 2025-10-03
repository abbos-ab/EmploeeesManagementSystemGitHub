using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDataColumnToFile3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedAt",
                table: "File",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "File");
        }
    }
}
