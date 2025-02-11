using System.Reflection.Emit;
using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class RentReceptionConfiguration : IEntityTypeConfiguration<RentReception>
    {
        public void Configure(EntityTypeBuilder<RentReception> builder)
        {

            // Relationship with Rent
            builder.HasOne(rr => rr.Rent)
                   .WithOne(r => r.RentReception)
                   .HasForeignKey<RentReception>(rr => rr.RentID)
                   .OnDelete(DeleteBehavior.Restrict);

            // ReturnDate default value
            builder.Property(rr => rr.ReturnDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
