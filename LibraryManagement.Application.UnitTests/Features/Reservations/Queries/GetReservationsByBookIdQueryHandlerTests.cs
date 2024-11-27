using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Reservations.Dto;
using LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByBookId;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Reservations.Queries
{
    public class GetReservationsByBookIdQueryHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IAppLogger<GetReservationsByBookIdQueryHandler>> _loggerMock;
        private readonly IMapper _mockMapper;

        public GetReservationsByBookIdQueryHandlerTests()
        {
            _reservationRepositoryMock = MockReservationRepository.GetMockReservationRepository();
            _loggerMock = new Mock<IAppLogger<GetReservationsByBookIdQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<ReservationProfile>();
            });

            _mockMapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetReservationsByBookIdQueryHandler_ReturnsMappedReservation_WhenReservationExists()
        {
            // Arrange
            var bookId = 1;
            var handler = new GetReservationsByBookIdQueryHandler(_reservationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetReservationByBookIdQuery(bookId), CancellationToken.None);
            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<ReservationDto>();
            result.BookId.ShouldBe(bookId);
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetReservationsByBookIdQueryHandler_ThrowsNotFoundException_WhenReservationDoesNotExist()
        {
            // Arrange
            var bookId = -99; // Non-existent BookId
            var handler = new GetReservationsByBookIdQueryHandler(_reservationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                handler.Handle(new GetReservationByBookIdQuery(bookId), CancellationToken.None));
            _loggerMock.Verify(log => log.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
