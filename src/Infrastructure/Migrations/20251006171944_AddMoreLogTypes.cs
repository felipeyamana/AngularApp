using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreLogTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LogTypesSet",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LogTypesSet",
                keyColumn: "Id",
                keyValue: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LogTypesSet",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 5, 19, 41, 32, 0, DateTimeKind.Utc), "Logs related to user actions and interactions.", true, "User Logs" },
                    { 2, new DateTime(2025, 10, 5, 19, 41, 32, 0, DateTimeKind.Utc), "Logs related to system operations and background tasks.", true, "System Logs" },
                    { 3, new DateTime(2025, 10, 5, 19, 41, 32, 0, DateTimeKind.Utc), "Logs related to authentication, authorization, and access control.", true, "Security Logs" },
                    { 4, new DateTime(2025, 10, 5, 19, 41, 32, 0, DateTimeKind.Utc), "Logs related to external integrations and API interactions.", true, "Integration Logs" }
                });
        }
    }
}
