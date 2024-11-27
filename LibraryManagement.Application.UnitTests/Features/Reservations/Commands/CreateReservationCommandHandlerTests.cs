using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Reservations.Commands.CreateReservation;
using LibraryManagement.Application.Features.Reservations.Dto;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using MediatR;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Reservations.Commands
{
    public class CreateReservationCommandHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAppLogger<CreateReservationCommandHandler>> _loggerMock;
        private readonly IMapper _mapper;

        public CreateReservationCommandHandlerTests()
        {
            _reservationRepositoryMock = MockReservationRepository.GetMockReservationRepository();
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<IAppLogger<CreateReservationCommandHandler>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ReservationProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task CreateReservationCommandHandler_ReturnsReservationDto_WhenValidRequest()
        {
            // Arrange
            var handler = new CreateReservationCommandHandler(
                _reservationRepositoryMock.Object,
                _mapper,
                _bookRepositoryMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object
            );

            var command = new CreateReservationCommand
            {
                BookId = 5,
                CustomerId = "c49140e8-6d3d-4b87-a88a-41d69c357560",
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<ReservationDto>();
            result.BookId.ShouldBe(command.BookId);
            result.CustomerId.ShouldBe(command.CustomerId);

            _reservationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Once);
            _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task CreateReservationCommandHandler_ThrowsBadRequestException_WhenBookIsNotAvailable()
        {
            // Arrange
            var handler = new CreateReservationCommandHandler(
               _reservationRepositoryMock.Object,
               _mapper,
               _bookRepositoryMock.Object,
               _mediatorMock.Object,
               _loggerMock.Object
           );

            var command = new CreateReservationCommand
            {
                BookId = 999, // Non-existent or unavailable book
                CustomerId = "c12345",
            };

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));

            _reservationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<ReservationCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        // Background task will ensure BookState and Reservations are consistent
        [Fact]
        public async Task CreateReservationCommandHandler_ThrowsBadRequestException_WhenReservationAlreadyExists()
        {
            // Arrange
            var handler = new CreateReservationCommandHandler(
               _reservationRepositoryMock.Object,
               _mapper,
               _bookRepositoryMock.Object,
               _mediatorMock.Object,
               _loggerMock.Object
           );

            var command = new CreateReservationCommand
            {
                BookId = 1,
                CustomerId = "existingCustomer",
            };

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));

            _reservationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<ReservationCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }


        [Fact]
        public async Task CreateReservationCommandHandler_LogsError_WhenDatabaseFails()
        {
            // Arrange
            var handler = new CreateReservationCommandHandler(
                _reservationRepositoryMock.Object,
                _mapper,
                _bookRepositoryMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object
            );

            var command = new CreateReservationCommand
            {
                BookId = 5,
                CustomerId = "existingCustomer",
            };

            _reservationRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Reservation>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Should.ThrowAsync<DatabaseException>(() => handler.Handle(command, CancellationToken.None));

            _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
            _reservationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Reservation>()), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<ReservationCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // Testin Validator file
        [Fact]
        public async Task CreateReservationCommandHandler_ThrowsBadRequestException_WhenValidationFails()
        {
            // Arrange
            var validator = new CreateReservationCommandValidator(
                _bookRepositoryMock.Object,
                _reservationRepositoryMock.Object
            );

            var command = new CreateReservationCommand
            {
                BookId = 0, // Invalid BookId
                CustomerId = "c12345",
            };

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.ErrorMessage.Contains("Book Id must be greater than 0"));
        }
    }
}

