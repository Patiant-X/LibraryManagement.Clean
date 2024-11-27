using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Notifications.Dto;
using LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByBookId;
using LibraryManagement.Application.MappingProfiles;
using LibraryManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;

namespace LibraryManagement.Application.UnitTests.Features.Notifications.Queries
{
    public class GetNotificationsByBookIdQueryHandlerTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IAppLogger<GetNotificationsByBookIdQueryHandler>> _loggerMock;
        private IMapper _mockMapper;

        public GetNotificationsByBookIdQueryHandlerTests()
        {
            _notificationRepositoryMock = MockNotificationRepository.GetMockNotificationRepository();
            _loggerMock = new Mock<IAppLogger<GetNotificationsByBookIdQueryHandler>>();

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<NotificationProfile>();
            });

            _mockMapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetNotificationsByBookIdQueryHandler_ReturnsMappedNotifications_WhenNotificationsExist()
        {
            // Arrange
            var bookId = 1;

            var handler = new GetNotificationsByBookIdQueryHandler(_notificationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetNotifcationsByBookIdQuery(bookId), CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<NotificationDto>>();
            result.Count.ShouldBeGreaterThan(0);
            _loggerMock.Verify(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetNotificationsByBookIdQueryHandler_ThrowsNotFoundException_WhenNotificationsDoNotExist()
        {
            // Arrange
            var bookId = -99;

            var handler = new GetNotificationsByBookIdQueryHandler(_notificationRepositoryMock.Object, _mockMapper, _loggerMock.Object);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                handler.Handle(new GetNotifcationsByBookIdQuery(bookId), CancellationToken.None));

            _loggerMock.Verify(log => log.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
