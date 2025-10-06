using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_LogTypes_LogTypeId",
                table: "Logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogTypes",
                table: "LogTypes");

            migrationBuilder.RenameTable(
                name: "LogTypes",
                newName: "LogTypesSet");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LogTypesSet",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "LogTypesSet",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogTypesSet",
                table: "LogTypesSet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_LogTypesSet_LogTypeId",
                table: "Logs",
                column: "LogTypeId",
                principalTable: "LogTypesSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_LogTypesSet_LogTypeId",
                table: "Logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogTypesSet",
                table: "LogTypesSet");

            migrationBuilder.DeleteData(
                table: "LogTypesSet",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LogTypesSet",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LogTypesSet",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LogTypesSet",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.RenameTable(
                name: "LogTypesSet",
                newName: "LogTypes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LogTypes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "LogTypes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogTypes",
                table: "LogTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_LogTypes_LogTypeId",
                table: "Logs",
                column: "LogTypeId",
                principalTable: "LogTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
