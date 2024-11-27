using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Notifications.Commands.CreateNotification;
using LibraryManagement.Application.Features.Notifications.Dto;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Notifications.Commands
{
    public class CreateNotificationCommandHandlerTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IAppLogger<CreateNotificationCommandHandler>> _mockLogger;
        private readonly IMapper _mapper;

        public CreateNotificationCommandHandlerTests()
        {
            _notificationRepositoryMock = MockNotificationRepository.GetMockNotificationRepository();
            _bookRepositoryMock = MockBookRepository.GetMockBooksRepository();
            _mockLogger = new Mock<IAppLogger<CreateNotificationCommandHandler>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NotificationProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task CreateNotificationCommandHandler_ReturnsNotificationDto_WhenValidRequest()
        {
            // Arrange
            var handler = new CreateNotificationCommandHandler(
                _notificationRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _mapper,
                _mockLogger.Object
            );

            var command = new CreateNotificationCommand
            {
                BookId = 4,
                CustomerId = "c49140e8-6d3d-4b87-a88a-41d69c357560"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<NotificationDto>();
            result.BookId.ShouldBe(command.BookId);
            result.CustomerId.ShouldBe(command.CustomerId);

            _notificationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Notification>()), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateNotificationCommandHandler_ThrowsBadRequestException_WhenBookExistsValidationFails()
        {
            // Arrange
            var handler = new CreateNotificationCommandHandler(
                _notificationRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _mapper,
                _mockLogger.Object
            );

            var command = new CreateNotificationCommand
            {
                BookId = 999, // Non-existent book
                CustomerId = "123"
            };

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));

            _notificationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Notification>()), Times.Never);
            _mockLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task CreateNotificationCommandHandler_ThrowsBadRequestException_WhenNotificationIsNotUniqueValidationFails()
        {
            // Arrange
            var validator = new CreateNotificationCommandValidator(
                _bookRepositoryMock.Object,
                _notificationRepositoryMock.Object
            );

            var command = new CreateNotificationCommand
            {
                BookId = 3,
                CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1",
            };

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.ErrorMessage.Contains("Notification already exists"));
        }

        [Fact]
        public async Task CreateNotificationCommandHandler_ThrowsBadRequestException_WhenBookIsAvaialableValidationFails()
        {
            // Arrange
            var handler = new CreateNotificationCommandHandler(
                _notificationRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _mapper,
                _mockLogger.Object
            );

            var command = new CreateNotificationCommand
            {
                BookId = 7,
                CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1",
            };

            // Act & Assert
            await Should.ThrowAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));

            _notificationRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Notification>()), Times.Never);
            _mockLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task CreateNotificationCommandHandler_ShouldLogError_WhenRepositoryExceptionOccurs()
        {
            // Arrange
            _notificationRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Notification>()))
                .ThrowsAsync(new DatabaseException("Database error"));

            var handler = new CreateNotificationCommandHandler(
                _notificationRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _mapper,
                _mockLogger.Object);

            var command = new CreateNotificationCommand
            {
                BookId = 4,
                CustomerId = "c49140e8-6d3d-4b87-a88a-41d69c357560"
            };

            // Act & Assert
            await Assert.ThrowsAsync<DatabaseException>(() => handler.Handle(command, CancellationToken.None));

            _mockLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
