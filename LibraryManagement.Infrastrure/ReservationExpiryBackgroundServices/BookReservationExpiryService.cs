using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


public class BookReservationExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopedFactory;

    public BookReservationExpiryService(IServiceScopeFactory scopeFactory)
    {
        _scopedFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _scopedFactory.CreateScope())
        {
            var bookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
            var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var logger = scope.ServiceProvider.GetRequiredService<IAppLogger<BookReservationExpiryService>>();
            var userServices = scope.ServiceProvider.GetRequiredService<IUserServices>();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {



                    var expiredReservations = await reservationRepository.GetExpiredReservationsAsync();

                    foreach (var reservation in expiredReservations)
                    {
                        await UpdateBookStateAsync(reservation, bookRepository, logger);
                        await NotifyUsersAsync(reservation, bookRepository, notificationRepository, emailSender, logger, userServices);
                        await reservationRepository.DeleteAsync(reservation);
                    }

                }
                catch (Exception ex)
                {
                    logger.LogWarning("An error occurred during BookReservationExpiryService execution.", ex);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

    public async Task UpdateBookStateAsync(Reservation reservation, IBookRepository bookRepository, IAppLogger<BookReservationExpiryService> logger)
    {
        try
        {
            var book = await bookRepository.GetByIdAsync(reservation.BookId);
            if (book != null)
            {
                book.IsReserved = false;
                book.ReturnDate = null;
                await bookRepository.UpdateAsync(book);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Error updating book state for reservation {reservation.Id}.", ex);
        }
    }

    public async Task NotifyUsersAsync(Reservation reservation, IBookRepository bookRepository,
        INotificationRepository notificationRepository, IEmailSender emailSender, IAppLogger<BookReservationExpiryService> logger, IUserServices userServices)
    {
        try
        {
            var notifications = await notificationRepository.GetActiveNotificationsByBookIdAsync(reservation.BookId);

            foreach (var notification in notifications)
            {
                var book = await bookRepository.GetByIdAsync(reservation.BookId);
                if (book != null)
                {
                    var user = await userServices.GetCustomer(notification.CustomerId);
                    await emailSender.SendEmail(new EmailMessage
                    {
                        To = user.Email, // Use actual email from notification
                        Subject = "Book Available",
                        Body = $"The book '{book.Title}' is now available for reservation."
                    });

                    notification.IsNotified = true;
                    await notificationRepository.UpdateAsync(notification);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning($"Error sending notification for reservation {reservation.Id}.", ex);
        }
    }
}