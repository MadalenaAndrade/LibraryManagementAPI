﻿// <auto-generated />
using System;
using EFCoreClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EFCoreClasses.Migrations
{
    [DbContext(typeof(LibraryDbContext))]
    [Migration("20250211163539_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("LibraryHub")
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EFCoreClasses.Models.Author", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("ID");

                    b.ToTable("Author", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Book", b =>
                {
                    b.Property<long>("SerialNumber")
                        .HasColumnType("bigint");

                    b.Property<decimal>("FinePerDay")
                        .HasColumnType("decimal(4,2)");

                    b.Property<int>("PublisherID")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<short>("Year")
                        .HasColumnType("smallint");

                    b.HasKey("SerialNumber");

                    b.HasIndex("PublisherID");

                    b.ToTable("Book", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookAuthor", b =>
                {
                    b.Property<long>("SerialNumber")
                        .HasColumnType("bigint");

                    b.Property<long>("AuthorID")
                        .HasColumnType("bigint");

                    b.HasKey("SerialNumber", "AuthorID");

                    b.HasIndex("AuthorID");

                    b.ToTable("BookAuthor", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCategory", b =>
                {
                    b.Property<long>("SerialNumber")
                        .HasColumnType("bigint");

                    b.Property<short>("CategoryID")
                        .HasColumnType("smallint");

                    b.HasKey("SerialNumber", "CategoryID");

                    b.HasIndex("CategoryID");

                    b.ToTable("BookCategory", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCondition", b =>
                {
                    b.Property<short>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("ID"));

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<decimal>("FineModifier")
                        .HasColumnType("decimal(3,2)");

                    b.HasKey("ID");

                    b.ToTable("BookCondition", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCopy", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<short>("BookConditionID")
                        .HasColumnType("smallint");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValueSql("N''");

                    b.Property<long>("SerialNumber")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("BookConditionID");

                    b.HasIndex("SerialNumber");

                    b.ToTable("BookCopy", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookStock", b =>
                {
                    b.Property<long>("SerialNumber")
                        .HasColumnType("bigint");

                    b.Property<short>("AvailableAmount")
                        .HasColumnType("smallint");

                    b.Property<short>("TotalAmount")
                        .HasColumnType("smallint");

                    b.HasKey("SerialNumber");

                    b.ToTable("BookStock", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Category", b =>
                {
                    b.Property<short>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<short>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("ID");

                    b.ToTable("Category", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Client", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Contact")
                        .HasColumnType("int");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<int>("NIF")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("ID");

                    b.ToTable("Client", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Publisher", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("ID");

                    b.ToTable("Publisher", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Rent", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<int>("BookCopyID")
                        .HasColumnType("int");

                    b.Property<int>("ClientID")
                        .HasColumnType("int");

                    b.Property<DateTime>("DueDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("DATEADD(dd, 7, GETDATE())");

                    b.Property<DateTime>("StartDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.HasKey("ID");

                    b.HasIndex("BookCopyID");

                    b.HasIndex("ClientID");

                    b.ToTable("Rent", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.RentReception", b =>
                {
                    b.Property<long>("RentID")
                        .HasColumnType("bigint");

                    b.Property<short>("ReceivedConditionID")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("ReturnDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<decimal>("TotalFine")
                        .HasColumnType("decimal(7,2)");

                    b.HasKey("RentID");

                    b.HasIndex("ReceivedConditionID");

                    b.ToTable("RentReception", "LibraryHub");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Book", b =>
                {
                    b.HasOne("EFCoreClasses.Models.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookAuthor", b =>
                {
                    b.HasOne("EFCoreClasses.Models.Author", "Author")
                        .WithMany("BookAuthors")
                        .HasForeignKey("AuthorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreClasses.Models.Book", "Book")
                        .WithMany("BookAuthors")
                        .HasForeignKey("SerialNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Book");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCategory", b =>
                {
                    b.HasOne("EFCoreClasses.Models.Category", "Category")
                        .WithMany("BookCategories")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreClasses.Models.Book", "Book")
                        .WithMany("BookCategories")
                        .HasForeignKey("SerialNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCopy", b =>
                {
                    b.HasOne("EFCoreClasses.Models.BookCondition", "BookCondition")
                        .WithMany("BookCopies")
                        .HasForeignKey("BookConditionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreClasses.Models.Book", "Book")
                        .WithMany("BookCopies")
                        .HasForeignKey("SerialNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("BookCondition");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookStock", b =>
                {
                    b.HasOne("EFCoreClasses.Models.Book", "Book")
                        .WithOne("BookStock")
                        .HasForeignKey("EFCoreClasses.Models.BookStock", "SerialNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Rent", b =>
                {
                    b.HasOne("EFCoreClasses.Models.BookCopy", "BookCopy")
                        .WithMany("Rents")
                        .HasForeignKey("BookCopyID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreClasses.Models.Client", "Client")
                        .WithMany("Rents")
                        .HasForeignKey("ClientID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookCopy");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("EFCoreClasses.Models.RentReception", b =>
                {
                    b.HasOne("EFCoreClasses.Models.BookCondition", "BookCondition")
                        .WithMany("RentReceptions")
                        .HasForeignKey("ReceivedConditionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EFCoreClasses.Models.Rent", "Rent")
                        .WithOne("RentReception")
                        .HasForeignKey("EFCoreClasses.Models.RentReception", "RentID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BookCondition");

                    b.Navigation("Rent");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Author", b =>
                {
                    b.Navigation("BookAuthors");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Book", b =>
                {
                    b.Navigation("BookAuthors");

                    b.Navigation("BookCategories");

                    b.Navigation("BookCopies");

                    b.Navigation("BookStock")
                        .IsRequired();
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCondition", b =>
                {
                    b.Navigation("BookCopies");

                    b.Navigation("RentReceptions");
                });

            modelBuilder.Entity("EFCoreClasses.Models.BookCopy", b =>
                {
                    b.Navigation("Rents");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Category", b =>
                {
                    b.Navigation("BookCategories");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Client", b =>
                {
                    b.Navigation("Rents");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Publisher", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("EFCoreClasses.Models.Rent", b =>
                {
                    b.Navigation("RentReception")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
