using LibraryManagement.Application.Features.Reservations.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByCustomerId
{
    public record GetCustomersReservationsQuery(string CustomerId) : IRequest<List<ReservationDto>>;
}
