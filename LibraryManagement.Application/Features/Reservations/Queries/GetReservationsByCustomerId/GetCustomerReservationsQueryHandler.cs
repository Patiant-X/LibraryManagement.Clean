using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Reservations.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByCustomerId
{
    public class GetCustomerReservationsQueryHandler : IRequestHandler<GetCustomersReservationsQuery, List<ReservationDto>>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<GetCustomerReservationsQueryHandler> _logger;

        public GetCustomerReservationsQueryHandler(IReservationRepository reservationRepository, IMapper mapper, IAppLogger<GetCustomerReservationsQueryHandler> logger)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ReservationDto>> Handle(GetCustomersReservationsQuery request, CancellationToken cancellationToken)
        {

            var customerReservations = await _reservationRepository.GetReservationsByCustomerIdAsync(request.CustomerId);

            if (customerReservations == null)
            {
                _logger.LogWarning("No reservations found for CustomerId: {CustomerId}", request.CustomerId);
                throw new NotFoundException(nameof(Reservations), request.CustomerId);
            }


            var data = _mapper.Map<List<ReservationDto>>(customerReservations);
            _logger.LogInformation("Mapping reservations for CustomerId: {CustomerId}", request.CustomerId);
            return data;
        }
    }
}
