using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Features.Reservations.Commands.DeleteBookReservationEvent;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Features.Reservations.Commands
{
    public class DeleteBookReservationsEventHandlerTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;

        public DeleteBookReservationsEventHandlerTests()
        {
            _reservationRepositoryMock = MockReservationRepository.GetMockReservationRepository();
        }

        [Fact]
        public async Task DeleteBookReservationsEventHandler_DeletesReservations_WhenEventIsTriggered()
        {
            // Arrange
            var handler = new DeleteBookReservationsEventHandler(_reservationRepositoryMock.Object);


            // Act
            await handler.Handle(new DeleteBookEvent(1, 123456789, "Introduction to C#"), CancellationToken.None);

            // Assert
            _reservationRepositoryMock.Verify(
                r => r.DeleteAsync(It.Is<Reservation>(res => res.BookId == 1)),
                Times.Once
            );
        }
    }
}

