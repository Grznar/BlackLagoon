using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackLagoon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPropertiesToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationOfUser",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NameOfUser",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationOfUser",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NameOfUser",
                table: "AspNetUsers");
        }
    }
}
