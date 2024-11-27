using LibraryManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.CustomerId)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(n => n.BookId)
               .IsRequired();

            builder.Property(n => n.IsNotified)
                .IsRequired();

            builder.Property(n => n.DateCreated)
                .HasColumnType("datetime");

            builder.Property(n => n.DateModified)
                .HasColumnType("datetime");
        }
    }
}
