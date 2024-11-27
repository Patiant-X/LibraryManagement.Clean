using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Books.Commands.DeleteBook;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using MediatR;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Books.Commands
{
    public class DeleteBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAppLogger<DeleteBookCommandHandler>> _mockAppLogger;
        private readonly DeleteBookCommandHandler _handler;

        public DeleteBookCommandHandlerTests()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _mediatorMock = new Mock<IMediator>();
            _mockAppLogger = new Mock<IAppLogger<DeleteBookCommandHandler>>();
            _handler = new DeleteBookCommandHandler(
               _bookRepositoryMock.Object,
               _mediatorMock.Object,
               _mockAppLogger.Object);
        }

        [Fact]
        public async Task DeleteBookCommandHandler_ShouldDeleteBook_AndPublishEvent()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { Id = bookId };

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId))
             .ReturnsAsync(book);

            _bookRepositoryMock.Setup(repo => repo.DeleteAsync(book))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(new DeleteBookCommand { Id = bookId }, CancellationToken.None);

            // Assert
            result.ShouldBe(Unit.Value);
            _bookRepositoryMock.Verify(repo => repo.DeleteAsync(book), Times.Once);
            _mockAppLogger.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBookCommandHandler_ShouldThrowNotFoundException_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = -111;
            var command = new DeleteBookCommand { Id = bookId };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            _bookRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Book>()), Times.Never);
            _mockAppLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBookCommandHandler_ShouldLogError_WhenRepositoryExceptionOccurs()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { Id = bookId };

            _bookRepositoryMock.Setup(repo => repo.GetByIdAsync(bookId))
               .ReturnsAsync(book);

            _bookRepositoryMock.Setup(repo => repo.DeleteAsync(book))
                .ThrowsAsync(new DatabaseException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<DatabaseException>(() => _handler.Handle(new DeleteBookCommand { Id = bookId }, CancellationToken.None));

            _mockAppLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
