using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EFCoreClasses.Models;

namespace EFCoreClasses.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // Relationships with Publisher
            builder.HasOne(b => b.Publisher)
                   .WithMany(p => p.Books)
                   .HasForeignKey(b => b.PublisherID);
        }
    }
}
