using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Domain;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, CreateBookDto>
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IAppLogger<CreateBookCommandHandler> _logger;

        public CreateBookCommandHandler(IMapper mapper,
            IBookRepository bookRepository, IAppLogger<CreateBookCommandHandler> logger)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
        }
        public async Task<CreateBookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateBookCommandValidator(_bookRepository);
            var validationResult = await validator.ValidateAsync(request);


            if (validationResult.Errors.Any())
            {
                _logger.LogWarning("Validation errors in create request for {0} - {1}",
                    nameof(Book), request.ISBN);
                throw new BadRequestException("Invalid Book data", validationResult);
            }

            var bookToCreate = _mapper.Map<Book>(request);

            try
            {
                await _bookRepository.CreateAsync(bookToCreate);
                _logger.LogInformation("Book with ISBN {0} created successfully.", bookToCreate.ISBN);

            }
            catch (Exception ex)
            {
                _logger.LogWarning("Database error occurred while creating book with ISBN: {0}. Exception: {1}",
                    bookToCreate.ISBN, ex.Message);

                throw new DatabaseException("An error occurred while processing the request.", ex);
            }

            var data = _mapper.Map<CreateBookDto>(bookToCreate);
            _logger.LogInformation("Book was created successfully: {0}", data.Title);


            return data;
        }
    }
}
