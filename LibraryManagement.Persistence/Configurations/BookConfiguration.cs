using LibraryManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Property(b => b.ReturnDate)
                .HasColumnType("datetime")
                .HasDefaultValue(null);

            builder.HasData(
                new Book
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    IsReserved = false,
                    IsBorrowed = false,
                    ISBN = 123456789,
                    ReturnDate = null
                });
        }
    }
}
