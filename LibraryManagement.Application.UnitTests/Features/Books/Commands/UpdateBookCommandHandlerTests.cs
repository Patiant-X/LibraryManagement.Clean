using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using MediatR;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Books.Commands
{
    public class UpdateBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAppLogger<UpdateBookCommandHandler>> _loggerMock;
        private readonly IMapper _mapper;

        public UpdateBookCommandHandlerTests()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<IAppLogger<UpdateBookCommandHandler>>();


            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<BookProfile>(); // Assume BookProfile contains necessary mappings
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task UpdateBookCommandHandler_ShouldUpdateBook_WhenValidationPasses_WhenEventIsPublished()
        {
            // Arrange
            //_bookRepositoryMock
            //    .Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
            //    .Returns((Task<Book>)Task.CompletedTask); // Simulate successful update

            var handler = new UpdateBookCommandHandler(_mapper, _bookRepositoryMock.Object, _mediatorMock.Object, _loggerMock.Object);

            var updateBookCommand = new UpdateBookCommand
            {
                Id = 1,
                Title = "Mastering C# Testing",
                ISBN = 987654321,
                IsReserved = false,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act
            var result = await handler.Handle(updateBookCommand, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<UpdateBookDto>();
            result.Title.ShouldBe("Mastering C# Testing");
            _bookRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
            _loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateBookCommandHandler_ShouldThrowBadRequestException_WhenValidationFails()
        {
            // Arrange
            var handler = new UpdateBookCommandHandler(_mapper, _bookRepositoryMock.Object, _mediatorMock.Object, _loggerMock.Object);

            var updateBookCommand = new UpdateBookCommand
            {
                Id = 1,
                Title = "Duplicate ISBN",
                ISBN = 567890123,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(updateBookCommand, CancellationToken.None));
            _bookRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Never);
            _loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookCommandHandler_ShouldNotAddEvent_WhenBookIsBorrowed()
        {
            // Arrange
            var book = new Book
            {
                Id = 2,
                Title = "Mastering ASP.NET Core",
                IsReserved = true,
                IsBorrowed = false,
                ISBN = 23456789,
                ReturnDate = null
            };

            _bookRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(book);

            _bookRepositoryMock
               .Setup(repo => repo.IsBookUnique(It.IsAny<int>()))
               .ReturnsAsync(true);

            //_bookRepositoryMock
            //    .Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
            //    .Returns((Task<Book>)Task.CompletedTask);

            var handler = new UpdateBookCommandHandler(_mapper, _bookRepositoryMock.Object, _mediatorMock.Object, _loggerMock.Object);

            var updateBookCommand = new UpdateBookCommand
            {
                Id = 3,
                Title = "Updated Book Title",
                ISBN = 23456789,
                IsReserved = true,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act
            var result = await handler.Handle(updateBookCommand, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            _loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookCommandHandler_ShouldNotAddEvent_WhenBookIsReserved()
        {
            // Arrange
            var book = new Book
            {
                Id = 1,
                Title = "Mastering C# Testing",
                IsReserved = true,
                IsBorrowed = true,
                ISBN = 123456789,
                ReturnDate = DateTime.Now
            };

            _bookRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(book);

            _bookRepositoryMock
                .Setup(repo => repo.IsBookUnique(It.IsAny<int>()))
                .ReturnsAsync(true); // Simulate book uniqueness check

            //_bookRepositoryMock
            //    .Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
            //    .Returns((Task<Book>)Task.CompletedTask); // Simulate successful update

            var handler = new UpdateBookCommandHandler(_mapper, _bookRepositoryMock.Object, _mediatorMock.Object, _loggerMock.Object);
            var updateBookCommand = new UpdateBookCommand
            {
                Id = 1,
                Title = "Mastering C# Testing",
                ISBN = 987654321,
                IsReserved = true,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act
            var result = await handler.Handle(updateBookCommand, CancellationToken.None);

            // Assert
            result.ShouldBeOfType<UpdateBookDto>();
            result.Title.ShouldBe("Mastering C# Testing");
            _bookRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
            _loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task UpdateBookCommandHandler_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
                .ThrowsAsync(new Exception("Database error"));

            var handler = new UpdateBookCommandHandler(_mapper, _bookRepositoryMock.Object, _mediatorMock.Object, _loggerMock.Object);

            var updateBookCommand = new UpdateBookCommand
            {
                Id = 1,
                Title = "Mastering C# Testing",
                ISBN = 987654321,
                IsReserved = false,
                IsBorrowed = false,
                ReturnDate = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<DatabaseException>(() => handler.Handle(updateBookCommand, CancellationToken.None));
            _loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }


    }


}
