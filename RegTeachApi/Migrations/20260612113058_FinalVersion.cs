using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegTeachApi.Migrations
{
    /// <inheritdoc />
    public partial class FinalVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "RegTeachUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEnd",
                table: "RegTeachUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetCode",
                table: "RegTeachUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetCodeExpiresAt",
                table: "RegTeachUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "RegTeachUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "RegTeachUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "RegTeachUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "RegTeachUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetCode",
                table: "RegTeachUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetCodeExpiresAt",
                table: "RegTeachUsers");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "RegTeachUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "RegTeachUsers");
        }
    }
}
