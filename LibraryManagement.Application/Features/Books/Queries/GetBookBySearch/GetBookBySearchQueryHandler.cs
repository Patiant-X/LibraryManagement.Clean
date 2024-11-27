using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetBooksBySearch
{
    public class GetBookBySearchQueryHandler : IRequestHandler<GetBookBySearchQuery, List<BookSearchDto>>
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IAppLogger<GetBookBySearchQueryHandler> _logger;

        public GetBookBySearchQueryHandler(IMapper mapper, IBookRepository bookRepository, IAppLogger<GetBookBySearchQueryHandler> logger)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task<List<BookSearchDto>> Handle(GetBookBySearchQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.SearchBookAsync(request.Title);

            if (books == null)
            {
                _logger.LogWarning($"No books found matching the title: {request.Title}");
                throw new NotFoundException(nameof(Books), request.Title);
            }

            _logger.LogInformation($"Found {books.Count} book(s) matching the title: {request.Title}");
            return _mapper.Map<List<BookSearchDto>>(books);
        }
    }
}
