using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Reservations.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByBookId
{
    public class GetReservationsByBookIdQueryHandler : IRequestHandler<GetReservationByBookIdQuery, ReservationDto>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<GetReservationsByBookIdQueryHandler> _logger;

        public GetReservationsByBookIdQueryHandler(IReservationRepository reservationRepository, IMapper mapper, IAppLogger<GetReservationsByBookIdQueryHandler> logger)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ReservationDto> Handle(GetReservationByBookIdQuery request, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepository.GetActiveReservationsByBookIdAsync(request.BookId);

            if (reservation == null)
            {
                _logger.LogWarning("No reservations found for BookId: {BookId}", request.BookId);
                throw new NotFoundException(nameof(Reservations), request.BookId);
            }

            var data = _mapper.Map<ReservationDto>(reservation);
            _logger.LogInformation("Mapping reservations for BookId: {BookId}", request.BookId);
            return data;

        }
    }
}
