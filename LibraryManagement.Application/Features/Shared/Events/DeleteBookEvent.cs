using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Features.Shared.Events
{
    public class DeleteBookEvent : IDomainEvent
    {

        public int BookId { get; set; }
        public int? ISBN { get; set; }
        public string? Title { get; set; }

        public DeleteBookEvent(int bookId, int isbn, string title)
        {
            BookId = bookId;
            ISBN = isbn;
            Title = title;
        }

    }
}
