using LibraryManagement.Application.Features.Reservations.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommand : IRequest<ReservationDto>
    {
        public int BookId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
    }
}
