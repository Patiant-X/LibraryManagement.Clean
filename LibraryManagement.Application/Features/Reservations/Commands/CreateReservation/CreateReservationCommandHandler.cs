using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Reservations.Dto;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Domain;
using MediatR;

namespace LibraryManagement.Application.Features.Reservations.Commands.CreateReservation
{
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ReservationDto>
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IMediator _mediator;
        private readonly IAppLogger<CreateReservationCommandHandler> _logger;

        public CreateReservationCommandHandler(IReservationRepository reservationRepository, IMapper mapper, IBookRepository bookRepository, IMediator mediator,
            IAppLogger<CreateReservationCommandHandler> logger)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _mediator = mediator;
            _logger = logger;
        }
        public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateReservationCommandValidator(_bookRepository, _reservationRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
            {
                _logger.LogWarning("Validation failed for Reservation creation for BookId: {BookId}, CustomerId: {CustomerId}. Errors: {Errors}", request.BookId, request.CustomerId, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                throw new BadRequestException("Invalid Reservation data", validationResult);
            }

            var reservationToCreate = _mapper.Map<Reservation>(request);

            try
            {

                await _reservationRepository.CreateAsync(reservationToCreate);

                // Add domain event to reservation entity
                reservationToCreate.AddDomainEvent(new ReservationCreatedEvent(request.BookId));

                // Log reservation creation success
                _logger.LogInformation("Reservation created successfully for BookId: {BookId}, CustomerId: {CustomerId}", request.BookId, request.CustomerId);

                var data = _mapper.Map<ReservationDto>(reservationToCreate);
                return data;
            }
            catch (Exception ex)
            {
                // Log database error
                _logger.LogWarning("An error occurred while creating reservation for BookId: {BookId}, CustomerId: {CustomerId}", request.BookId, request.CustomerId, ex);
                throw new DatabaseException("Error occurred while saving reservation data", ex);
            }
        }
    }
}
