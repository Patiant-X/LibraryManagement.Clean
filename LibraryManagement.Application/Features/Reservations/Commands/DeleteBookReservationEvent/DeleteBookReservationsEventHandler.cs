using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Features.Shared.Events;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Commands.DeleteBookReservationEvent
{
    public class DeleteBookReservationsEventHandler : INotificationHandler<DeleteBookEvent>
    {
        private readonly IReservationRepository _reservationRepository;

        public DeleteBookReservationsEventHandler(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        public async Task Handle(DeleteBookEvent notification, CancellationToken cancellationToken)
        {

            await _reservationRepository.DeleteAsync(new Domain.Reservation { BookId = notification.BookId });
            return;
        }
    }
}
