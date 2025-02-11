using System.Reflection.Emit;
using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class RentConfiguration : IEntityTypeConfiguration<Rent>
    {
        public void Configure(EntityTypeBuilder<Rent> builder)
        {
            // StartDate and DueDate default values
            builder.Property(r => r.StartDate)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.DueDate)
                   .HasDefaultValueSql("DATEADD(dd, 7, GETDATE())");
        }
    }
}
