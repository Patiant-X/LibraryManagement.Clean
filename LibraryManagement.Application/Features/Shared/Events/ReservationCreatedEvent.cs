using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Features.Shared.Events
{
    public class ReservationCreatedEvent : IDomainEvent
    {
        public int BookId { get; set; }

        public ReservationCreatedEvent(int bookId)
        {
            BookId = bookId;
        }
    }
}
