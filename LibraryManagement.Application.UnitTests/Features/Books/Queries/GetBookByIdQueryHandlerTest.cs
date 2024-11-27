using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Books.Queries.GetBookDetails;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Books.Queries
{

    public class GetBookByIdQueryHandlerTest
    {

        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IAppLogger<GetBookDetailsQueryHandler>> _loggerMock;
        private readonly IMapper _mapper;

        public GetBookByIdQueryHandlerTest()
        {
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _loggerMock = new Mock<IAppLogger<GetBookDetailsQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<BookProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetBookDetailsQueryHandler_ShouldReturnBookDetails()
        {
            // Arrange
            var handler = new GetBookDetailsQueryHandler(_mapper, _bookRepositoryMock.Object, _loggerMock.Object);
            var bookDetailsQuery = new GetBookDetailsQuery(4);

            // Act
            var result = await handler.Handle(bookDetailsQuery, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<BookDetailsDto>();
            result.Id.ShouldBe(4); // mock data contains a book with Id = 1
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetBookDetailsQueryHandler_ShouldThrowNotFoundException_WhenBookIdDoesNotExist()
        {
            // Arrange
            var query = new GetBookDetailsQuery(-1); // Non-existent book ID

            _bookRepositoryMock
                .Setup(repo => repo.GetByIdAsync(query.Id))
                .ReturnsAsync((Book?)null); // Simulate book not found

            var handler = new GetBookDetailsQueryHandler(_mapper, _bookRepositoryMock.Object, _loggerMock.Object);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
            _loggerMock.Verify(log => log.LogWarning(It.IsAny<string>()), Times.Once);
        }
    }
}
