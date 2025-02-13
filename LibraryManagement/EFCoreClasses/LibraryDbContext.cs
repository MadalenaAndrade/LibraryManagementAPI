using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EFCoreClasses.Models;

namespace EFCoreClasses
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookStock> BookStocks { get; set; }
        public DbSet<BookCondition> BookConditions { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<RentReception> RentReceptions { get; set; }

        // Fluent API used for schema, keys, relationships and default values; data annotations used for field properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // schema
            modelBuilder.HasDefaultSchema("LibraryHub");

            // Apply all entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
        }
    }
}
