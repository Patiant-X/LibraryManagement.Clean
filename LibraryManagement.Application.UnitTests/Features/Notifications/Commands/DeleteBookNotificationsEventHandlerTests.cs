using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Features.Notifications.Commands
{
    public class DeleteBookNotificationsEventHandlerTests
    {
        [Fact]
        public async Task Handle_DeletesNotifications_WhenBookIsUnavailable()
        {
            // Arrange
            var mockNotificationRepo = MockNotificationRepository.GetMockNotificationRepository();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserServices = new Mock<IUserServices>();

            mockNotificationRepo.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(It.IsAny<int>()))
                                .ReturnsAsync(new List<Notification>
                                {
                                    new Notification { CustomerId ="c49140e8-6d3d-4b87-a88a-41d69c357560", BookId = 1, IsNotified = false }
                                });

            mockEmailSender.Setup(sender => sender.SendEmail(It.IsAny<EmailMessage>()))
                           .ReturnsAsync(true);

            mockUserServices.Setup(user => user.GetCustomer(It.IsAny<string>()))
                           .ReturnsAsync(new Models.Identity.Customer
                           {
                               Email = "UserEmail"
                           });

            var handler = new Application.Features.Notifications.Commands.DeleteBookNotificationEvent.DeleteBookNotificationsEventHandler(mockNotificationRepo.Object, mockEmailSender.Object, mockUserServices.Object);
            var notificationEvent = new Application.Features.Shared.Events.DeleteBookEvent(1, 123456, "Test Book");

            // Act
            await handler.Handle(notificationEvent, CancellationToken.None);

            // Assert
            mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
            mockNotificationRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Notification>()), Times.Once);
            mockUserServices.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesNothing_WhenNoActiveNotificationsFound()
        {
            // Arrange
            var mockNotificationRepo = MockNotificationRepository.GetMockNotificationRepository();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserServices = new Mock<IUserServices>();

            mockNotificationRepo.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(It.IsAny<int>()))
                                .ReturnsAsync((List<Notification>)null);

            var handler = new Application.Features.Notifications.Commands.DeleteBookNotificationEvent.DeleteBookNotificationsEventHandler(mockNotificationRepo.Object, mockEmailSender.Object, mockUserServices.Object);
            var notificationEvent = new Application.Features.Shared.Events.DeleteBookEvent(1, 123456, "Test Book");

            // Act
            await handler.Handle(notificationEvent, CancellationToken.None);

            // Assert
            mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
            mockNotificationRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Notification>()), Times.Never);
            mockUserServices.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Never);
        }


    }
}
