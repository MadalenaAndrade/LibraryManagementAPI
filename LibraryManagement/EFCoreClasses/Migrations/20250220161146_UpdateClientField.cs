using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreClasses.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Client_NIF",
                schema: "LibraryHub",
                table: "Client",
                column: "NIF",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Client_NIF",
                schema: "LibraryHub",
                table: "Client");
        }
    }
}
