using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Features.Books.Commands.ReserveBook;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Features.Books.Commands
{
    public class ReservationCreatedEventHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IAppLogger<ReservationCreatedEventHandler>> _loggerMock;
        private readonly ReservationCreatedEventHandler _handler;

        public ReservationCreatedEventHandlerTests()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _loggerMock = new Mock<IAppLogger<ReservationCreatedEventHandler>>();
            _handler = new ReservationCreatedEventHandler(_bookRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ReservationCreatedEventHandler_ShouldReserveBook_WhenBookIsFound()
        {
            // Arrange
            var bookId = 1;
            var reservationEvent = new ReservationCreatedEvent(bookId);
            var book = new Book { Id = bookId };

            _bookRepositoryMock
                .Setup(repo => repo.ReserveBookAsync(bookId))
                .ReturnsAsync(book);

            // Act
            await _handler.Handle(reservationEvent, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.ReserveBookAsync(bookId), Times.Once);
            _loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task ReservationCreatedEventHandler_ShouldLogWarning_WhenBookNotFound()
        {
            // Arrange
            var bookId = 1;
            var reservationEvent = new ReservationCreatedEvent(bookId);

            _bookRepositoryMock
                .Setup(repo => repo.ReserveBookAsync(bookId))
                .ReturnsAsync((Book)null);

            // Act
            await _handler.Handle(reservationEvent, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(repo => repo.ReserveBookAsync(bookId), Times.Once);
            _loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task ReservationCreatedEventHandler_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var bookId = 1;
            var reservationEvent = new ReservationCreatedEvent(bookId);

            _bookRepositoryMock
                .Setup(repo => repo.ReserveBookAsync(bookId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await _handler.Handle(reservationEvent, CancellationToken.None);
            _loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
