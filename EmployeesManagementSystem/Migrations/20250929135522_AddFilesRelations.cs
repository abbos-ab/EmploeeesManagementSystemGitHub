using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "File",
                type: "longblob",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "receiverId",
                table: "File",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_File_receiverId",
                table: "File",
                column: "receiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_File_Users_receiverId",
                table: "File",
                column: "receiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Users_receiverId",
                table: "File");

            migrationBuilder.DropIndex(
                name: "IX_File_receiverId",
                table: "File");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "File");

            migrationBuilder.DropColumn(
                name: "receiverId",
                table: "File");
        }
    }
}
