using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Application.UnitTests.Mocks;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Features.Notifications.Commands
{
    public class BookAvailableNotificationEventHandler
    {
        [Fact]
        public async Task BookAvailableNotificationEventHandler_SendsNotifications_WhenBookIsAvailable()
        {
            // Arrange
            var mockNotificationRepo = MockNotificationRepository.GetMockNotificationRepository();
            var mockBookRepo = MockBookRepository.GetMockBooksRepository();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserServices>();

            mockBookRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync(new Book { Id = 1, Title = "Test Book", IsReserved = false });

            mockNotificationRepo.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(It.IsAny<int>()))
                                .ReturnsAsync(new List<Notification>
                                {
                            new Notification { CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1", BookId = 1, IsNotified = false }
                                });

            mockEmailSender.Setup(sender => sender.SendEmail(It.IsAny<EmailMessage>()))
                           .ReturnsAsync(true);

            mockUserService.Setup(user => user.GetCustomer(It.IsAny<string>()))
                          .ReturnsAsync(new Models.Identity.Customer
                          {
                              Email = "UserEmail"
                          });

            var handler = new Application.Features.Notifications.Commands.SendNotification.BookAvailableNotificationEventHandler(mockNotificationRepo.Object, mockEmailSender.Object, mockBookRepo.Object, mockUserService.Object);
            var notificationEvent = new Application.Features.Shared.Events.BookAvailableEvent(1);

            // Act
            await handler.Handle(notificationEvent, CancellationToken.None);

            // Assert
            mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
            mockNotificationRepo.Verify(repo => repo.UpdateAsync(It.Is<Notification>(n => n.IsNotified)), Times.Once);
            mockUserService.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Once);
        }

        [Fact]
        public async Task BookAvailableNotificationEventHandler_DoesNotUpdateNotification_WhenEmailIsNotSent()
        {
            // Arrange
            var mockNotificationRepo = MockNotificationRepository.GetMockNotificationRepository();
            var mockBookRepo = MockBookRepository.GetMockBooksRepository();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserServices>();

            mockBookRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync(new Book { Id = 1, Title = "Test Book", IsReserved = false });

            mockNotificationRepo.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(It.IsAny<int>()))
                                .ReturnsAsync(new List<Notification>
                                {
                            new Notification { CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1", BookId = 1, IsNotified = false }
                                });

            mockEmailSender.Setup(sender => sender.SendEmail(It.IsAny<EmailMessage>()))
                           .ReturnsAsync(false);
            mockUserService.Setup(user => user.GetCustomer(It.IsAny<string>()))
                         .ReturnsAsync(new Models.Identity.Customer
                         {
                             Email = "UserEmail"
                         });

            var handler = new Application.Features.Notifications.Commands.SendNotification.BookAvailableNotificationEventHandler(mockNotificationRepo.Object, mockEmailSender.Object, mockBookRepo.Object, mockUserService.Object);
            var notificationEvent = new Application.Features.Shared.Events.BookAvailableEvent(1);

            // Act
            await handler.Handle(notificationEvent, CancellationToken.None);

            // Assert
            mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Once);
            mockNotificationRepo.Verify(repo => repo.UpdateAsync(It.Is<Notification>(n => n.IsNotified)), Times.Never);
            mockUserService.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Once);
        }

        [Fact]
        public async Task BookAvailableNotificationEventHandler_DoesNothing_WhenBookDoesNotExist()
        {
            // Arrange
            var mockNotificationRepo = MockNotificationRepository.GetMockNotificationRepository();
            var mockBookRepo = MockBookRepository.GetMockBooksRepository();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserServices>();

            mockBookRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync((Book)null);

            var handler = new Application.Features.Notifications.Commands.SendNotification.BookAvailableNotificationEventHandler(mockNotificationRepo.Object, mockEmailSender.Object, mockBookRepo.Object, mockUserService.Object);
            var notificationEvent = new Application.Features.Shared.Events.BookAvailableEvent(1);

            // Act
            await handler.Handle(notificationEvent, CancellationToken.None);

            // Assert
            mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
            mockNotificationRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Notification>()), Times.Never);
            mockUserService.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Never);
        }

        [Fact]
        public async Task BookAvailableNotificationEventHandler_DoesNothing_WhenNotificationDoesNotExist()
        {
            // Arrange
            var mockNotificationRepo = MockNotificationRepository.GetMockNotificationRepository();
            var mockBookRepo = MockBookRepository.GetMockBooksRepository();
            var mockEmailSender = new Mock<IEmailSender>();
            var mockUserService = new Mock<IUserServices>();

            mockBookRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                       .ReturnsAsync(new Book { Id = 1, Title = "Test Book", IsReserved = false });

            mockNotificationRepo.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(It.IsAny<int>()))
                               .ReturnsAsync((List<Notification>?)null);

            mockEmailSender.Setup(sender => sender.SendEmail(It.IsAny<EmailMessage>()))
                           .ReturnsAsync(true);

            var handler = new Application.Features.Notifications.Commands.SendNotification.BookAvailableNotificationEventHandler(mockNotificationRepo.Object, mockEmailSender.Object, mockBookRepo.Object, mockUserService.Object);
            var notificationEvent = new Application.Features.Shared.Events.BookAvailableEvent(1);

            // Act
            await handler.Handle(notificationEvent, CancellationToken.None);

            // Assert
            mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
            mockNotificationRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Notification>()), Times.Never);
            mockUserService.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Never);
        }

    }

}
