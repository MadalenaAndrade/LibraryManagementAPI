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


        // Applies default configurations only if the context has not been configured externally.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=WAACHAN\\\\SQLEXPRESS;Database=LibraryCodeFirst;Trusted_Connection=True;TrustServerCertificate=True"; // This is my local connection string, but it can be overridden by the Web API (from appsettings).
                optionsBuilder.UseSqlServer(connectionString);
            }
            // Enable Lazy Loading by default (this part is always applied)
            optionsBuilder.UseLazyLoadingProxies();
        }


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
