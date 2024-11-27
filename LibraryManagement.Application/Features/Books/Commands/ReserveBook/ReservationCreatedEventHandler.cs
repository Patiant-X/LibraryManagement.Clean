using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Features.Shared.Events;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Commands.ReserveBook
{
    public class ReservationCreatedEventHandler : INotificationHandler<ReservationCreatedEvent>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAppLogger<ReservationCreatedEventHandler> _logger;

        public ReservationCreatedEventHandler(IBookRepository bookRepository, IAppLogger<ReservationCreatedEventHandler> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;

        }
        public async Task Handle(ReservationCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookRepository.ReserveBookAsync(notification.BookId);

                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookId} could not be reserved. It might not exist or could be already reserved.", notification.BookId);
                    return;
                }

                _logger.LogInformation("Book with ID {BookId} has been successfully reserved.", notification.BookId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("An error occurred while reserving the book with ID {BookId}.", notification.BookId, ex);
            }
        }

    }
}
