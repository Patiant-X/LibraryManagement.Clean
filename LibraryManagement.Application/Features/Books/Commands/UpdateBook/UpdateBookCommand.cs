using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommand : IRequest<UpdateBookDto>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ISBN { get; set; }
        public bool IsBorrowed { get; set; }
        public bool IsReserved { get; set; }
        public DateTime? ReturnDate { get; set; } = null;
    }
}
