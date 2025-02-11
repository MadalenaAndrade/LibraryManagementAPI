using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class BookCategoryConfiguration : IEntityTypeConfiguration<BookCategory>
    {
        public void Configure(EntityTypeBuilder<BookCategory> builder)
        {
            // PK
            builder.HasKey(bc => new { bc.SerialNumber, bc.CategoryID });

            // Relationship with Category
            builder.HasOne(bc => bc.Category)
                   .WithMany(c => c.BookCategories)
                   .HasForeignKey(bc => bc.CategoryID);

            // Relationship with Book
            builder.HasOne(bc => bc.Book)
                   .WithMany(b => b.BookCategories)
                   .HasForeignKey(bc => bc.SerialNumber);
        }
    }
}
