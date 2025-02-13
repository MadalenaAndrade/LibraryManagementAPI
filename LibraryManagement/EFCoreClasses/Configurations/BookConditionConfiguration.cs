// Created this seed data, to populate on the first migration the table BookCondition that has only four imutable entries

using EFCoreClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreClasses.Configurations
{
    public class BookConditionConfiguration : IEntityTypeConfiguration<BookCondition>
    {
        public void Configure(EntityTypeBuilder<BookCondition> builder)
        {
            // to avoid data duplication on migration I turned the field "ID" unique though its index and introduced it manually
            builder.HasIndex(bc => bc.ID)
                   .IsUnique();

            builder.HasData( 
                new BookCondition
                {
                    ID = 1,
                    Condition = "As new",
                    FineModifier = 1m
                },
                new BookCondition
                {
                    ID = 2,
                    Condition = "Good",
                    FineModifier = 0.75m
                },
                new BookCondition
                {
                    ID = 3,
                    Condition = "Used",
                    FineModifier = 0.5m
                },
                new BookCondition
                {
                    ID = 4,
                    Condition = "Bad",
                    FineModifier = 0.25m
                }
            );
        }
    }
}
