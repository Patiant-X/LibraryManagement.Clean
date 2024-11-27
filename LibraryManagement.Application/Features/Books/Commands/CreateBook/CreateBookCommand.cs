using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommand : IRequest<CreateBookDto>
    {
        public string Title { get; set; } = string.Empty;
        public int ISBN { get; set; }
        public bool IsReserved { get; set; } = false;
        public bool IsBorrowed { get; set; } = false;
        public DateTime? ReturnDate { get; set; } = null;
    }
}
