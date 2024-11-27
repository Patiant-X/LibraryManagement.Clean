using LibraryManagement.Domain;
using LibraryManagement.Domain.Common;
using LibraryManagement.Persistence.DatabaseContext;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace LibraryManagement.Persistence.IntergrationTests
{
    public class LmDatabaseContextTests
    {
        private Mock<IPublisher> _publisherMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private LmDatabaseContext _dbContext;

        public LmDatabaseContextTests()
        {
            var dbOptions = new DbContextOptionsBuilder<LmDatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _publisherMock = new Mock<IPublisher>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _dbContext = new LmDatabaseContext(dbOptions, _publisherMock.Object, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Save_ShouldSetDateCreatedAndDateModified()
        {
            // Arrange
            var book = new Book
            {
                Title = "Introduction to C#",
                IsReserved = false,
                IsBorrowed = false,
                ISBN = 123456789,
                ReturnDate = null
            };

            // Act
            await _dbContext.books.AddAsync(book);
            await _dbContext.SaveChangesAsync();

            // Assert
            book.DateCreated.ShouldNotBeNull();
            book.DateModified.ShouldNotBeNull();
        }

        [Fact]
        public async Task Save_ShouldClearDomainEventsAfterSaving()
        {
            // Arrange
            var book = new Book
            {
                Title = "Introduction to C#",
                IsReserved = false,
                IsBorrowed = false,
                ISBN = 123456789,
                ReturnDate = null
            };

            book.AddDomainEvent(new Mock<IDomainEvent>().Object);

            await _dbContext.books.AddAsync(book);

            // Act
            await _dbContext.SaveChangesAsync();

            // Assert
            book.DomainEvents.ShouldBeEmpty();
        }

        [Fact]
        public async Task Save_ShouldPublishDomainEvents_WhenUserIsOnline()
        {
            // Arrange
            var book = new Book { Title = "Introduction to C#", IsReserved = false, IsBorrowed = false, ISBN = 123456789, ReturnDate = null };
            var domainEvent = new Mock<IDomainEvent>().Object;
            book.AddDomainEvent(domainEvent);

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext)null);

            await _dbContext.books.AddAsync(book);

            // Act
            await _dbContext.SaveChangesAsync();

            // Assert
            _publisherMock.Verify(p => p.Publish(domainEvent, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Save_ShouldQueueDomainEvents_WhenUserIsOffline()
        {
            // Arrange
            var book = new Book
            {
                Title = "Introduction to C#",
                IsReserved = false,
                IsBorrowed = false,
                ISBN = 123456789,
                ReturnDate = null
            };
            var domainEvent = new Mock<IDomainEvent>().Object;
            book.AddDomainEvent(domainEvent);

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());

            await _dbContext.books.AddAsync(book);

            // Act
            await _dbContext.SaveChangesAsync();

            // Assert
            _httpContextAccessorMock.Verify(a => a.HttpContext!.Items["DomainEventsQueue"], Times.Never);
        }

        [Fact]
        public async Task Save_ShouldNotThrow_WhenNoDomainEventsExist()
        {
            // Arrange
            var book = new Book
            {
                Title = "Introduction to C#",
                IsReserved = false,
                IsBorrowed = false,
                ISBN = 123456789,
                ReturnDate = null
            };
            await _dbContext.books.AddAsync(book);

            // Act
            var exception = await Record.ExceptionAsync(() => _dbContext.SaveChangesAsync());

            // Assert
            exception.ShouldBeNull();
        }

    }
}