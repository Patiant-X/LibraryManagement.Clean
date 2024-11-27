using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain
{
    public class Reservation : BaseEntity
    {
        public int BookId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        /// <summary>
        /// Reservations expire 24 hours after creation.
        /// </summary>
        public bool IsExpired => DateCreated.HasValue && DateCreated.Value.AddHours(24) <= DateTime.UtcNow;
    }
}
