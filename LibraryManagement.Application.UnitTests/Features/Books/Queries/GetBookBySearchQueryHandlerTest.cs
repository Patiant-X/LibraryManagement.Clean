using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Books.Queries.GetBooksBySearch;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Books.Queries
{
    public class GetBookBySearchQueryHandlerTest
    {

        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IAppLogger<GetBookBySearchQueryHandler>> _loggerMock;
        private readonly IMapper _mapper;

        public GetBookBySearchQueryHandlerTest()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _loggerMock = new Mock<IAppLogger<GetBookBySearchQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<BookProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetBookBySearchQueryHandler_ShouldReturnMatchingBooks()
        {
            // Arrange
            var handler = new GetBookBySearchQueryHandler(_mapper, _bookRepositoryMock.Object, _loggerMock.Object);
            var searchQuery = new GetBookBySearchQuery("Introduction");

            // Act
            var result = await handler.Handle(searchQuery, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<BookSearchDto>>();
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetBookBySearchQueryHandler_ShouldThrowNotFoundException_WhenNoMatchingBooksFound()
        {
            // Arrange
            var searchQuery = new GetBookBySearchQuery("NonExistentTitle");

            _bookRepositoryMock
                .Setup(repo => repo.SearchBookAsync(searchQuery.Title))
                .ReturnsAsync((List<Book>)null); // Simulate no books found

            var handler = new GetBookBySearchQueryHandler(_mapper, _bookRepositoryMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(searchQuery, CancellationToken.None));
            _loggerMock.Verify(log => log.LogWarning(It.IsAny<string>()), Times.Once);
        }

    }


}