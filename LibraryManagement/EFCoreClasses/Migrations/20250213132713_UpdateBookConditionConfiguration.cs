using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EFCoreClasses.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookConditionConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "LibraryHub",
                table: "BookCondition",
                columns: new[] { "ID", "Condition", "FineModifier" },
                values: new object[,]
                {
                    { (short)1, "As new", 1m },
                    { (short)2, "Good", 0.75m },
                    { (short)3, "Used", 0.5m },
                    { (short)4, "Bad", 0.25m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCondition_ID",
                schema: "LibraryHub",
                table: "BookCondition",
                column: "ID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookCondition_ID",
                schema: "LibraryHub",
                table: "BookCondition");

            migrationBuilder.DeleteData(
                schema: "LibraryHub",
                table: "BookCondition",
                keyColumn: "ID",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                schema: "LibraryHub",
                table: "BookCondition",
                keyColumn: "ID",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                schema: "LibraryHub",
                table: "BookCondition",
                keyColumn: "ID",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                schema: "LibraryHub",
                table: "BookCondition",
                keyColumn: "ID",
                keyValue: (short)4);
        }
    }
}
