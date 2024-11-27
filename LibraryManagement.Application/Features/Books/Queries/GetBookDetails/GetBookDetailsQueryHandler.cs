using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetBookDetails
{
    public class GetBookDetailsQueryHandler : IRequestHandler<GetBookDetailsQuery, BookDetailsDto>
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IAppLogger<GetBookDetailsQueryHandler> _logger;

        public GetBookDetailsQueryHandler(IMapper mapper, IBookRepository bookRepository, IAppLogger<GetBookDetailsQueryHandler> logger)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task<BookDetailsDto> Handle(GetBookDetailsQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id);

            if (book == null)
            {
                _logger.LogWarning($"Book with ID {request.Id} not found.");
                throw new NotFoundException(nameof(Books), request.Id);
            }

            _logger.LogInformation($"Book with ID {request.Id} retrieved successfully.");
            return _mapper.Map<BookDetailsDto>(book);
        }
    }
}
