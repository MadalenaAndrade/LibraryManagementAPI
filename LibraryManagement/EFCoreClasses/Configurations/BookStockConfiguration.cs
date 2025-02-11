using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class BookStockConfiguration : IEntityTypeConfiguration<BookStock>
    {
        public void Configure(EntityTypeBuilder<BookStock> builder)
        {
            // Relationship with Book
            builder.HasOne(bs => bs.Book)
                   .WithOne(b => b.BookStock)
                   .HasForeignKey<BookStock>(bs => bs.SerialNumber);
        }
    }
}
