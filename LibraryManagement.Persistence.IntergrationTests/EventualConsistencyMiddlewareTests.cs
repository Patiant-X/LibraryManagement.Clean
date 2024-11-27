using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Domain.Common;
using LibraryManagement.Persistence.DatabaseContext;
using LibraryManagement.Persistence.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LibraryManagement.Persistence.IntergrationTests
{
    public class EventualConsistencyMiddlewareTests
    {
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<IAppLogger<EventualConsistencyMiddleware>> _mockAppLogger;
        private readonly Mock<LmDatabaseContext> _mockDbContext;
        private readonly EventualConsistencyMiddleware _middleware;

        public EventualConsistencyMiddlewareTests()
        {
            _mockPublisher = new Mock<IPublisher>();
            _mockAppLogger = new Mock<IAppLogger<EventualConsistencyMiddleware>>();
            _mockDbContext = new Mock<LmDatabaseContext>();
            _middleware = new EventualConsistencyMiddleware(ctx => Task.CompletedTask);

            // Mock database transaction behavior
            _mockDbContext.Setup(db => db.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());
        }

        [Fact]
        public async Task EventualConsistencyMiddleware_ShouldPublishDomainEvents_AndCommitTransaction()
        {
            // Arrange
            var domainEventsQueue = new Queue<IDomainEvent>();
            domainEventsQueue.Enqueue(Mock.Of<IDomainEvent>());

            var context = new DefaultHttpContext();
            context.Items["DomainEventsQueue"] = domainEventsQueue;

            // Act
            await _middleware.InvokeAsync(context, _mockPublisher.Object, _mockDbContext.Object, _mockAppLogger.Object);

            // Simulate response completion
            context.Response.OnCompleted(() => Task.CompletedTask);

            // Assert
            _mockPublisher.Verify(p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockAppLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task EventualConsistencyMiddleware_ShouldLogWarning_OnPublishException()
        {
            // Arrange
            _mockPublisher.Setup(p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            var domainEventsQueue = new Queue<IDomainEvent>();
            domainEventsQueue.Enqueue(Mock.Of<IDomainEvent>());

            var context = new DefaultHttpContext();
            context.Items["DomainEventsQueue"] = domainEventsQueue;

            // Act
            await _middleware.InvokeAsync(context, _mockPublisher.Object, _mockDbContext.Object, _mockAppLogger.Object);
            context.Response.OnCompleted(() => Task.CompletedTask);

            // Assert
            _mockAppLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
