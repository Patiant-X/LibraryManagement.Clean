using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Mocks
{
    public class MockNotificationRepository
    {
        public static Mock<INotificationRepository> GetMockNotificationRepository()
        {
            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = 1,
                    BookId = 1,
                    CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1",
                    IsNotified = false,
                    DateCreated = DateTime.UtcNow.AddDays(-1)
                },
                new Notification
                {
                    Id = 2,
                    BookId = 2,
                    CustomerId = "de810ce8-8bbd-404d-85b9-9b9711b694fd",
                    IsNotified = true,
                    DateCreated = DateTime.UtcNow.AddDays(-3)
                },
                new Notification
                {
                    Id = 3,
                    BookId = 3,
                    CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1",
                    IsNotified = false,
                    DateCreated = DateTime.UtcNow.AddDays(-2)
                },
                new Notification
                {
                    Id = 4,
                    BookId = 1,
                    CustomerId = "c49140e8-6d3d-4b87-a88a-41d69c357560",
                    IsNotified = true,
                    DateCreated = DateTime.UtcNow.AddDays(-5)
                }
            };

            var mockRepo = new Mock<INotificationRepository>();

            mockRepo.Setup(r => r.GetActiveNotificationsByBookIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int bookId) =>
             {
                 var filteredNotifications = notifications.Where(n => n.BookId == bookId && !n.IsNotified).ToList();
                 return (filteredNotifications.Any() ? filteredNotifications : null);
             });

            // Mock GetNotificationsByCustomerIdAsync
            mockRepo.Setup(r => r.GetNotificationsByCustomerIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string customerId) =>
                {
                    var filteredNotifications = notifications.Where(n => n.CustomerId == customerId).ToList();
                    return (filteredNotifications.Any() ? filteredNotifications : null);
                });

            // Mock IsNotificationUnique
            mockRepo.Setup(r => r.IsNotificationUnique(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((int bookId, string customerId) =>
                    !notifications.Any(n => n.BookId == bookId && n.CustomerId == customerId));

            // Mock CreateAsync
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Notification>())).Callback((Notification notification) =>
            {
                notification.Id = notifications.Max(n => n.Id) + 1; // Auto-increment Id
                notification.DateCreated = DateTime.Now;
                notification.IsNotified = false;
                notifications.Add(notification);
            });

            // Mock UpdateAsync
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Notification>())).Callback((Notification notification) =>
            {
                var existingNotification = notifications.FirstOrDefault(n => n.Id == notification.Id);
                if (existingNotification != null)
                {
                    notifications.Remove(existingNotification);
                    notification.DateModified = DateTime.Now;
                    notifications.Add(notification);
                }
            });

            // Mock DeleteAsync
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Notification>())).Callback((Notification notification) =>
            {
                var existingNotification = notifications.FirstOrDefault(n => n.Id == notification.Id);
                if (existingNotification != null)
                {
                    notifications.Remove(existingNotification);
                }
            });

            return mockRepo;
        }
    }
}
