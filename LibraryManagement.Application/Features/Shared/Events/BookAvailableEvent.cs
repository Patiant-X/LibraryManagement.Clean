using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Features.Shared.Events
{
    public class BookAvailableEvent : IDomainEvent
    {
        public int BookId { get; set; }

        public BookAvailableEvent(int bookId)
        {
            BookId = bookId;
        }
    }
}
