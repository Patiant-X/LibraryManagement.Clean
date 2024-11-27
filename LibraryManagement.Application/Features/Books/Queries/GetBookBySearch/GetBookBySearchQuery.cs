using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetBooksBySearch
{
    public record GetBookBySearchQuery(string Title) : IRequest<List<BookSearchDto>>;
}
