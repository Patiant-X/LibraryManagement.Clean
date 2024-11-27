namespace LibraryManagement.Application.Features.Reservations.Dto
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public DateTime? DateCreated { get; set; }
        /// <summary>
        /// Reservations expire 24 hours after creation.
        /// </summary>
        public bool IsExpired => DateCreated.HasValue && DateCreated.Value.AddHours(24) <= DateTime.Now;
    }
}
