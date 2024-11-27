using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Domain;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Commands.SendNotification
{
    public class BookAvailableNotificationEventHandler : INotificationHandler<BookAvailableEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailSender _emailSender;
        private readonly IBookRepository _bookRepository;
        private readonly IUserServices _userServices;

        public BookAvailableNotificationEventHandler(INotificationRepository notificationRepository, IEmailSender emailSender, IBookRepository bookRepository, IUserServices userServices)
        {
            _notificationRepository = notificationRepository;
            _emailSender = emailSender;
            _bookRepository = bookRepository;
            _userServices = userServices;
        }
        public async Task Handle(BookAvailableEvent notification, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(notification.BookId);
            if (book == null) return;
            var notifications = await _notificationRepository.GetActiveNotificationsByBookIdAsync(notification.BookId);
            if (notifications == null) return;

            foreach (var item in notifications)
            {
                if (!item.IsNotified)
                {
                    // Fetch the users email from the database using Identity once Identity has been setup.
                    var user = await _userServices.GetCustomer(item.CustomerId);

                    var email = new EmailMessage
                    {
                        To = user.Email,
                        Subject = book.Title + "Is Available for reservation",
                        Body = "The book is available"
                    };

                    var notificationState = await _emailSender.SendEmail(email);

                    if (notificationState)
                    {
                        var newNotification = new Notification
                        {
                            IsNotified = true,
                            CustomerId = item.CustomerId,
                            BookId = item.BookId,

                        };
                        await _notificationRepository.UpdateAsync(newNotification);
                    };
                }
            }

            return;

        }
    }
}
