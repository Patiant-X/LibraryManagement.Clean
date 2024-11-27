using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Shared.Events;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.DeleteBook
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMediator _mediator;
        private readonly IAppLogger<DeleteBookCommandHandler> _logger;

        public DeleteBookCommandHandler(IBookRepository bookRepository,
            IMediator mediator, IAppLogger<DeleteBookCommandHandler> logger)
        {
            _bookRepository = bookRepository;
            _mediator = mediator;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var bookToDelete = await _bookRepository.GetByIdAsync(request.Id);
            var oldBook = bookToDelete;

            if (bookToDelete == null)
            {
                _logger.LogWarning("Book not found", request.Id);
                throw new NotFoundException(nameof(Books), request.Id);

            }
            try
            {
                await _bookRepository.DeleteAsync(bookToDelete);
                oldBook.AddDomainEvent(new DeleteBookEvent(oldBook.Id, oldBook.ISBN, oldBook.Title));
                _logger.LogInformation("Book with ID {0} deleted successfully.", request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Database error occurred while deleting book with ID {0}. Exception: {1}", request.Id, ex.Message);
                throw new DatabaseException("An error occurred while processing the request.", ex);
            }

            return Unit.Value;
        }
    }
}
