using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Books.Commands
{
    public class CreateBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private IMapper _mockMapper;
        private Mock<IAppLogger<CreateBookCommandHandler>> _mockAppLogger;

        public CreateBookCommandHandlerTests()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<BookProfile>();
            });

            _mockMapper = mapperConfig.CreateMapper();
            _mockAppLogger = new Mock<IAppLogger<CreateBookCommandHandler>>();
        }

        [Fact]
        public async Task CreateBookCommandHandler_ShouldCreateBook_WhenValidationPasses()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(repo => repo.IsBookUnique(It.IsAny<int>()))
                .ReturnsAsync(true); // Simulate that the book is unique
                                     //    _bookRepositoryMock
                                     //        .Setup(repo => repo.CreateAsync(It.IsAny<Book>()))
                                     //.Returns(Task.CompletedTask); // Simulate book creation


            var handler = new CreateBookCommandHandler(_mockMapper, _bookRepositoryMock.Object, _mockAppLogger.Object);

            var createBookCommand = new CreateBookCommand
            {
                Title = "Test Book",
                ISBN = 0123456, // Valid 13-digit ISBN
                IsReserved = false,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act
            var result = await handler.Handle(createBookCommand, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<CreateBookDto>();
            result.Title.ShouldBe("Test Book");
            _bookRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookCommandHandler_ShouldThrowBadRequestException_WhenValidationFails()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(repo => repo.IsBookUnique(It.IsAny<int>()))
                .ReturnsAsync(false); // Simulate that the book is unique

            var handler = new CreateBookCommandHandler(_mockMapper, _bookRepositoryMock.Object, _mockAppLogger.Object);

            var createBookCommand = new CreateBookCommand
            {
                Title = "Entity Framework Core Essentials",
                IsReserved = false,
                IsBorrowed = false,
                ISBN = 567890123,
                ReturnDate = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(createBookCommand, CancellationToken.None));
        }

        [Fact]
        public async Task CreateBookCommandHandler_ShouldLogError_WhenRepositoryExceptionOccurs()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Book>()))
                .ThrowsAsync(new DatabaseException("Database error"));

            var handler = new CreateBookCommandHandler(_mockMapper, _bookRepositoryMock.Object, _mockAppLogger.Object);

            var createBookCommand = new CreateBookCommand
            {
                Title = "Test Book",
                ISBN = 0123456,
                IsReserved = false,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<DatabaseException>(() => handler.Handle(createBookCommand, CancellationToken.None));

            _mockAppLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

    }
}
