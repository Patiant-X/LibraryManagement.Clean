using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Application.Models.Identity;
using LibraryManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Moq;

public class BookReservationExpiryServiceTests
{
    private readonly Mock<IBookRepository> _mockBookRepository = new Mock<IBookRepository>();
    private readonly Mock<IReservationRepository> _mockReservationRepository = new Mock<IReservationRepository>();
    private readonly Mock<INotificationRepository> _mockNotificationRepository = new Mock<INotificationRepository>();
    private readonly Mock<IEmailSender> _mockEmailSender = new Mock<IEmailSender>();
    private readonly Mock<IAppLogger<BookReservationExpiryService>> _mockLogger = new Mock<IAppLogger<BookReservationExpiryService>>();
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
    private readonly Mock<IServiceScope> _mockServiceScope = new Mock<IServiceScope>();
    private readonly Mock<IServiceProvider> _mockServiceProvider = new Mock<IServiceProvider>();
    private readonly Mock<IUserServices> _mockUserServices = new Mock<IUserServices>();                 // Mock the service provider

    private readonly BookReservationExpiryService _service;

    public BookReservationExpiryServiceTests()
    {
        // Set up the service provider to return the required services
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IBookRepository)))
            .Returns(_mockBookRepository.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IReservationRepository)))
            .Returns(_mockReservationRepository.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(INotificationRepository)))
            .Returns(_mockNotificationRepository.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEmailSender)))
            .Returns(_mockEmailSender.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IUserServices)))
            .Returns(_mockUserServices.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IAppLogger<BookReservationExpiryService>)))
            .Returns(_mockLogger.Object);

        // Set up the service scope to return the mocked service provider
        _mockServiceScope.Setup(x => x.ServiceProvider)
                         .Returns(_mockServiceProvider.Object);

        // Mock the IServiceScopeFactory to return the mocked scope
        _mockServiceScopeFactory.Setup(x => x.CreateScope())
                                .Returns(_mockServiceScope.Object);


        // Initialize the service with the mocked scope factory
        _service = new BookReservationExpiryService(_mockServiceScopeFactory.Object);
    }

    [Fact]
    public async Task UpdateBookStateAsync_ShouldUpdateBookState_WhenBookExists()
    {
        // Arrange
        var reservation = new Reservation { Id = 1, BookId = 101 };
        var book = new Book { Id = 101, IsReserved = false, ReturnDate = null };

        _mockBookRepository.Setup(repo => repo.GetByIdAsync(reservation.BookId))
                           .ReturnsAsync(book);

        //_mockBookRepository.Setup(repo => repo.UpdateAsync(book))
        //                   .Returns((Task<Book>)Task.CompletedTask); // Simulate the update operation

        // Act
        await _service.UpdateBookStateAsync(reservation, _mockBookRepository.Object, _mockLogger.Object);

        // Assert
        Assert.False(book.IsReserved);
        Assert.Null(book.ReturnDate);
        _mockBookRepository.Verify(repo => repo.UpdateAsync(book), Times.Once);
    }

    [Fact]
    public async Task UpdateBookStateAsync_ShouldDoNothing_WhenBookDoesNotExist()
    {
        // Arrange
        var reservation = new Reservation { Id = 1, BookId = 101 };

        _mockBookRepository.Setup(repo => repo.GetByIdAsync(reservation.BookId))
                           .ReturnsAsync((Book)null);

        // Act
        await _service.UpdateBookStateAsync(reservation, _mockBookRepository.Object, _mockLogger.Object);

        // Assert
        _mockBookRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task UpdateBookStateAsync_ShouldLogWarning_OnException()
    {
        // Arrange
        var reservation = new Reservation { Id = 1, BookId = 101 };

        _mockBookRepository.Setup(repo => repo.GetByIdAsync(reservation.BookId))
                           .ThrowsAsync(new System.Exception("Database error"));

        // Act
        await _service.UpdateBookStateAsync(reservation, _mockBookRepository.Object, _mockLogger.Object);

        // Assert
        _mockLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public async Task NotifyUsersAsync_ShouldSendEmails_WhenNotificationsExist()
    {
        // Arrange
        var reservation = new Reservation { Id = 1, BookId = 101 };
        var book = new Book { Id = 101, Title = "Test Book" };
        var notifications = new List<Notification>
        {
            new Notification { Id = 1, BookId = 101, IsNotified = false }
        };

        _mockNotificationRepository.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(reservation.BookId))
                                   .ReturnsAsync(notifications);
        _mockBookRepository.Setup(repo => repo.GetByIdAsync(reservation.BookId))
                           .ReturnsAsync(book);
        _mockUserServices.Setup(user => user.GetCustomer(It.IsAny<string>()))
                        .ReturnsAsync(new Customer
                        {
                            Email = "UserEmail"
                        });

        // Act
        await _service.NotifyUsersAsync(reservation, _mockBookRepository.Object, _mockNotificationRepository.Object, _mockEmailSender.Object, _mockLogger.Object, _mockUserServices.Object);

        // Assert
        _mockEmailSender.Verify(sender => sender.SendEmail(It.Is<EmailMessage>(email => email.Body.Contains("Test Book"))), Times.Once);
        _mockNotificationRepository.Verify(repo => repo.UpdateAsync(It.Is<Notification>(n => n.IsNotified)), Times.Once);
        _mockUserServices.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Once);
    }

    [Fact]
    public async Task NotifyUsersAsync_ShouldDoNothing_WhenNoNotificationsExist()
    {
        // Arrange
        var reservation = new Reservation { Id = 1, BookId = 101 };

        _mockNotificationRepository.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(reservation.BookId))
                                   .ReturnsAsync(new List<Notification>());

        // Act
        await _service.NotifyUsersAsync(reservation, _mockBookRepository.Object, _mockNotificationRepository.Object, _mockEmailSender.Object, _mockLogger.Object, _mockUserServices.Object);

        // Assert
        _mockEmailSender.Verify(sender => sender.SendEmail(It.IsAny<EmailMessage>()), Times.Never);
        _mockNotificationRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Notification>()), Times.Never);
        _mockUserServices.Verify(user => user.GetCustomer(It.IsAny<String>()), Times.Never);
    }

    [Fact]
    public async Task NotifyUsersAsync_ShouldLogWarning_OnException()
    {
        // Arrange
        var reservation = new Reservation { Id = 1, BookId = 101 };
        var notifications = new List<Notification>
        {
            new Notification { Id = 1, BookId = 101, IsNotified = false }
        };

        _mockNotificationRepository.Setup(repo => repo.GetActiveNotificationsByBookIdAsync(reservation.BookId))
                                   .ReturnsAsync(notifications);
        _mockBookRepository.Setup(repo => repo.GetByIdAsync(reservation.BookId))
                           .ThrowsAsync(new System.Exception("Database error"));

        // Act
        await _service.NotifyUsersAsync(reservation, _mockBookRepository.Object, _mockNotificationRepository.Object, _mockEmailSender.Object, _mockLogger.Object, _mockUserServices.Object);

        // Assert
        _mockLogger.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }
}


