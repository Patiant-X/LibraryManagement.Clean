using LibraryManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Persistence.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.CustomerId)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(r => r.BookId)
                .IsRequired();

            builder.Property(r => r.DateCreated)
                .HasColumnType("datetime");

            builder.Property(r => r.DateModified)
                .HasColumnType("datetime");
        }
    }
}
