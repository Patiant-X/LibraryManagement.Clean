using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Domain;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, UpdateBookDto>
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IMediator _mediator;
        private readonly IAppLogger<UpdateBookCommandHandler> _logger;

        public UpdateBookCommandHandler(IMapper mapper, IBookRepository bookRepository, IMediator mediator, IAppLogger<UpdateBookCommandHandler> logger)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _mediator = mediator;
            _logger = logger;

        }
        public async Task<UpdateBookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateBookCommandValidator(_bookRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
            {
                _logger.LogWarning("Validation failed for Book ID {BookId}: {Errors}", request.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException("Invalid Book data", validationResult);
            }

            var bookToUpdate = _mapper.Map<Book>(request);

            try
            {
                await _bookRepository.UpdateAsync(bookToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("An error occurred while updating the book with ID {BookId}.", request.Id, ex);
                throw new DatabaseException("Error occurred while updating the book.", ex);
            }

            if (!bookToUpdate.IsBorrowed && !bookToUpdate.IsReserved && bookToUpdate.ReturnDate == null)
            {
                bookToUpdate.AddDomainEvent(new BookAvailableEvent(bookToUpdate.Id));
                _logger.LogInformation("Book with ID {BookId} is now available and event has been added.", bookToUpdate.Id);
            }

            var data = _mapper.Map<UpdateBookDto>(bookToUpdate);
            _logger.LogInformation("Book with ID {BookId} updated successfully.", bookToUpdate.Id);

            return data;
        }
    }
}
