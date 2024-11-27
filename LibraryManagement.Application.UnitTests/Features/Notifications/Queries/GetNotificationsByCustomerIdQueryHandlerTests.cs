using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Notifications.Dto;
using LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByCustomerId;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Notifications.Queries
{
    public class GetNotificationsByCustomerIdQueryHandlerTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IAppLogger<GetNotificationsByCustomerIdQueryHandler>> _loggerMock;
        private readonly IMapper _mockMapper;

        public GetNotificationsByCustomerIdQueryHandlerTests()
        {
            _notificationRepositoryMock = MockNotificationRepository.GetMockNotificationRepository();
            _loggerMock = new Mock<IAppLogger<GetNotificationsByCustomerIdQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<NotificationProfile>();
            });

            _mockMapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetNotificationsByCustomerIdQueryHandler_ReturnsMappedNotifications_WhenNotificationsExist()
        {
            // Arrange
            var customerId = "417732bf-58b9-45ad-a416-28446c65b7c1";
            var handler = new GetNotificationsByCustomerIdQueryHandler(_notificationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetNotificationsByCustomerIdQuery(customerId), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<NotificationDto>>();
            result.Count.ShouldBeGreaterThan(0);
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetNotificationsByCustomerIdQueryHandler_ThrowsNotFoundException_WhenNotificationsDoNotExist()
        {
            // Arrange
            var customerId = "123";

            var handler = new GetNotificationsByCustomerIdQueryHandler(_notificationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                handler.Handle(new GetNotificationsByCustomerIdQuery(customerId), CancellationToken.None));
            _loggerMock.Verify(log => log.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
