using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetBookDetails
{
    public record GetBookDetailsQuery(int Id) : IRequest<BookDetailsDto>;
}
