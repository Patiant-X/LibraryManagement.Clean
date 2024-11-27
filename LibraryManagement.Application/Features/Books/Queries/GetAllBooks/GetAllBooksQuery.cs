using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks
{
    public record GetAllBooksQuery : IRequest<List<BookDto>>;
}
