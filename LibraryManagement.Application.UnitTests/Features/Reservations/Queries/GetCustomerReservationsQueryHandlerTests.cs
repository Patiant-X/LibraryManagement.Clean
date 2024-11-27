using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Reservations.Dto;
using LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByCustomerId;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Reservations.Queries
{
    public class GetCustomerReservationsQueryHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IAppLogger<GetCustomerReservationsQueryHandler>> _loggerMock;
        private readonly IMapper _mockMapper;

        public GetCustomerReservationsQueryHandlerTests()
        {
            _reservationRepositoryMock = MockReservationRepository.GetMockReservationRepository();
            _loggerMock = new Mock<IAppLogger<GetCustomerReservationsQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<ReservationProfile>();
            });

            _mockMapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetCustomerReservationsQueryHandler_ReturnsMappedReservations_WhenReservationsExist()
        {
            // Arrange
            var customerId = "417732bf-58b9-45ad-a416-28446c65b7c1";
            var handler = new Application.Features.Reservations.Queries.GetReservationsByCustomerId.GetCustomerReservationsQueryHandler(_reservationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetCustomersReservationsQuery(customerId), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<ReservationDto>>();
            result.Count.ShouldBeGreaterThan(0);
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetCustomerReservationsQueryHandler_ThrowsNotFoundException_WhenReservationsDoNotExist()
        {
            // Arrange
            var customerId = "non-existent-customer-id"; // Non-existent CustomerId
            var handler = new Application.Features.Reservations.Queries.GetReservationsByCustomerId.GetCustomerReservationsQueryHandler(_reservationRepositoryMock.Object, _mockMapper, _loggerMock.Object);
            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                handler.Handle(new GetCustomersReservationsQuery(customerId), CancellationToken.None));
            _loggerMock.Verify(log => log.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
