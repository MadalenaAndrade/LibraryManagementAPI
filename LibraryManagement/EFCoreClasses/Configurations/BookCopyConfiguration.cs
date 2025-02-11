using System.Reflection.Emit;
using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
    {
        public void Configure(EntityTypeBuilder<BookCopy> builder)
        {
            //Notes default value
            builder.Property(bcp => bcp.Notes)
                   .HasDefaultValueSql("N''");
        }
    }
}
