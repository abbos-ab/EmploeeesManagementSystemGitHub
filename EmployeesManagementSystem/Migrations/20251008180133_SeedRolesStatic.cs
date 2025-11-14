using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeesManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesStatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "SuperAdmin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Admin" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

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
    }
}
