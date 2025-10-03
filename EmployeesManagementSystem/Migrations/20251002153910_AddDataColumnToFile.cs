using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDataColumnToFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Users_receiverId",
                table: "File");

            migrationBuilder.RenameColumn(
                name: "receiverId",
                table: "File",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_File_receiverId",
                table: "File",
                newName: "IX_File_ReceiverId");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "File",
                type: "longblob",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_File_Users_ReceiverId",
                table: "File",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Users_ReceiverId",
                table: "File");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "File");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "File",
                newName: "receiverId");

            migrationBuilder.RenameIndex(
                name: "IX_File_ReceiverId",
                table: "File",
                newName: "IX_File_receiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_File_Users_receiverId",
                table: "File",
                column: "receiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
