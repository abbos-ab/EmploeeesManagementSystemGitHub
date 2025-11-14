using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("87289805-8c0a-4357-a800-8af3a108a494"), "Admin" },
                    { new Guid("aef6c5cb-dd0a-4d5d-a342-ba23dbbadd87"), "User" },
                    { new Guid("db9c285c-c713-4efc-85ab-836972a93108"), "SuperAdmin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("87289805-8c0a-4357-a800-8af3a108a494"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("aef6c5cb-dd0a-4d5d-a342-ba23dbbadd87"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("db9c285c-c713-4efc-85ab-836972a93108"));
        }
    }
}
