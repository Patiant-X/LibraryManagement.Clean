using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, List<BookDto>>
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IAppLogger<GetAllBooksQueryHandler> _logger;

        public GetAllBooksQueryHandler(IMapper mapper, IBookRepository bookRepository, IAppLogger<GetAllBooksQueryHandler> logger)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task<List<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var allBooks = await _bookRepository.GetAllAsync();

            if (allBooks == null)
            {
                _logger.LogWarning("No books found in the repository.");
                throw new NotFoundException();
            }

            var data = _mapper.Map<List<BookDto>>(allBooks);
            _logger.LogInformation("Successfully fetched and mapped {Count} books.", data.Count);

            return data;
        }
    }
}
