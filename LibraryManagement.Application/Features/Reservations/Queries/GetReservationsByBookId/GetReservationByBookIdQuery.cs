using LibraryManagement.Application.Features.Reservations.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByBookId
{
    public record GetReservationByBookIdQuery(int BookId) : IRequest<ReservationDto>;
}
