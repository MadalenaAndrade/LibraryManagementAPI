using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            // PK
            builder.HasKey(ba => new { ba.SerialNumber, ba.AuthorID });

            //Relationship with Author
            builder.HasOne(ba => ba.Author)
                   .WithMany(a => a.BookAuthors)
                   .HasForeignKey(ba => ba.AuthorID);

            //Rekationship with Book
            builder.HasOne(ba => ba.Book)
                   .WithMany(b => b.BookAuthors)
                   .HasForeignKey(ba => ba.SerialNumber);
        }

    }
}
