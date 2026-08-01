using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "text",
                nullable: false,
                comment: "ASP.NET Identity PasswordHasher hashed password - never store plaintext",
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "BCrypt hashed password - never store plaintext");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "text",
                nullable: false,
                comment: "BCrypt hashed password - never store plaintext",
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "ASP.NET Identity PasswordHasher hashed password - never store plaintext");
        }
    }
}
