using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreClasses.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LibraryHub");

            migrationBuilder.CreateTable(
                name: "Author",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BookCondition",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Condition = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FineModifier = table.Column<decimal>(type: "decimal(3,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCondition", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    NIF = table.Column<int>(type: "int", nullable: false),
                    Contact = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Publisher",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publisher", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Book",
                schema: "LibraryHub",
                columns: table => new
                {
                    SerialNumber = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    FinePerDay = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    PublisherID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "FK_Book_Publisher_PublisherID",
                        column: x => x.PublisherID,
                        principalSchema: "LibraryHub",
                        principalTable: "Publisher",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthor",
                schema: "LibraryHub",
                columns: table => new
                {
                    SerialNumber = table.Column<long>(type: "bigint", nullable: false),
                    AuthorID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthor", x => new { x.SerialNumber, x.AuthorID });
                    table.ForeignKey(
                        name: "FK_BookAuthor_Author_AuthorID",
                        column: x => x.AuthorID,
                        principalSchema: "LibraryHub",
                        principalTable: "Author",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthor_Book_SerialNumber",
                        column: x => x.SerialNumber,
                        principalSchema: "LibraryHub",
                        principalTable: "Book",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCategory",
                schema: "LibraryHub",
                columns: table => new
                {
                    SerialNumber = table.Column<long>(type: "bigint", nullable: false),
                    CategoryID = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCategory", x => new { x.SerialNumber, x.CategoryID });
                    table.ForeignKey(
                        name: "FK_BookCategory_Book_SerialNumber",
                        column: x => x.SerialNumber,
                        principalSchema: "LibraryHub",
                        principalTable: "Book",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCategory_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalSchema: "LibraryHub",
                        principalTable: "Category",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCopy",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNumber = table.Column<long>(type: "bigint", nullable: false),
                    BookConditionID = table.Column<short>(type: "smallint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "N''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopy", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookCopy_BookCondition_BookConditionID",
                        column: x => x.BookConditionID,
                        principalSchema: "LibraryHub",
                        principalTable: "BookCondition",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCopy_Book_SerialNumber",
                        column: x => x.SerialNumber,
                        principalSchema: "LibraryHub",
                        principalTable: "Book",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookStock",
                schema: "LibraryHub",
                columns: table => new
                {
                    SerialNumber = table.Column<long>(type: "bigint", nullable: false),
                    TotalAmount = table.Column<short>(type: "smallint", nullable: false),
                    AvailableAmount = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookStock", x => x.SerialNumber);
                    table.ForeignKey(
                        name: "FK_BookStock_Book_SerialNumber",
                        column: x => x.SerialNumber,
                        principalSchema: "LibraryHub",
                        principalTable: "Book",
                        principalColumn: "SerialNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rent",
                schema: "LibraryHub",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    BookCopyID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "DATEADD(dd, 7, GETDATE())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rent", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rent_BookCopy_BookCopyID",
                        column: x => x.BookCopyID,
                        principalSchema: "LibraryHub",
                        principalTable: "BookCopy",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rent_Client_ClientID",
                        column: x => x.ClientID,
                        principalSchema: "LibraryHub",
                        principalTable: "Client",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentReception",
                schema: "LibraryHub",
                columns: table => new
                {
                    RentID = table.Column<long>(type: "bigint", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ReceivedConditionID = table.Column<short>(type: "smallint", nullable: false),
                    TotalFine = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentReception", x => x.RentID);
                    table.ForeignKey(
                        name: "FK_RentReception_BookCondition_ReceivedConditionID",
                        column: x => x.ReceivedConditionID,
                        principalSchema: "LibraryHub",
                        principalTable: "BookCondition",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentReception_Rent_RentID",
                        column: x => x.RentID,
                        principalSchema: "LibraryHub",
                        principalTable: "Rent",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_PublisherID",
                schema: "LibraryHub",
                table: "Book",
                column: "PublisherID");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_AuthorID",
                schema: "LibraryHub",
                table: "BookAuthor",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_BookCategory_CategoryID",
                schema: "LibraryHub",
                table: "BookCategory",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopy_BookConditionID",
                schema: "LibraryHub",
                table: "BookCopy",
                column: "BookConditionID");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopy_SerialNumber",
                schema: "LibraryHub",
                table: "BookCopy",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Rent_BookCopyID",
                schema: "LibraryHub",
                table: "Rent",
                column: "BookCopyID");

            migrationBuilder.CreateIndex(
                name: "IX_Rent_ClientID",
                schema: "LibraryHub",
                table: "Rent",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_RentReception_ReceivedConditionID",
                schema: "LibraryHub",
                table: "RentReception",
                column: "ReceivedConditionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthor",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "BookCategory",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "BookStock",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "RentReception",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "Author",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "Rent",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "BookCopy",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "Client",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "BookCondition",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "Book",
                schema: "LibraryHub");

            migrationBuilder.DropTable(
                name: "Publisher",
                schema: "LibraryHub");
        }
    }
}
