using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Books.Queries
{
    public class GetALlBooksListQueryHandlerTest : GetBookBySearchQueryHandlerTest
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private IMapper _mapper;
        private readonly Mock<IAppLogger<GetAllBooksQueryHandler>> _loggerMock;

        public GetALlBooksListQueryHandlerTest()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _loggerMock = new Mock<IAppLogger<GetAllBooksQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<BookProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetAllBooksQueryHandler_ShouldReturnAllBooks()
        {
            // Arrange
            var handler = new GetAllBooksQueryHandler(_mapper, _bookRepositoryMock.Object, _loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetAllBooksQuery(), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<BookDto>>();
            result.Count.ShouldBeGreaterThan(0);
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetAllBooksQueryHandler_ShouldThrowNotFoundException_WhenNoBooksAreFound()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync((List<Book>)null); // Simulate no books found

            var handler = new GetAllBooksQueryHandler(_mapper, _bookRepositoryMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetAllBooksQuery(), CancellationToken.None));
            _loggerMock.Verify(log => log.LogWarning("No books found in the repository."), Times.Once);
        }
    }
}
