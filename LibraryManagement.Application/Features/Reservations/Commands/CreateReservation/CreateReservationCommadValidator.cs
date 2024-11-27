using FluentValidation;
using LibraryManagement.Application.Contracts.Persistence;

namespace LibraryManagement.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IReservationRepository _reservationRepository;

        public CreateReservationCommandValidator(IBookRepository bookRepository, IReservationRepository reservationRepository)
        {
            RuleFor(x => x.BookId)
               .NotEmpty().WithMessage("{PropertyName} is required")
               .NotNull()
               .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
               .MustAsync(BookMustBeAvailable).WithMessage("{PropertyName} resource not available or not found");

            RuleFor(x => x)
                .MustAsync(ReservationMustNotExist).WithMessage("Reservation for this Book already Exists");

            _bookRepository = bookRepository;
            _reservationRepository = reservationRepository;
        }

        private async Task<bool> ReservationMustNotExist(CreateReservationCommand command, CancellationToken token)
        {
            var reservation = await _reservationRepository.GetActiveReservationsByBookIdAsync(command.BookId);

            if (reservation == null)
                return true;
            else if (reservation.IsExpired)
                return true;
            else return reservation == null;
        }

        private async Task<bool> BookMustBeAvailable(int bookId, CancellationToken token)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return false;
            else if (book.IsReserved || book.IsBorrowed)
                return false;
            else
                return true;
        }
    }
}
