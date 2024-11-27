using LibraryManagement.Application.Contracts.Email;
using LibraryManagement.Application.Contracts.Identity;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Features.Shared.Events;
using LibraryManagement.Application.Models.Email;
using LibraryManagement.Domain;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Commands.DeleteBookNotificationEvent
{
    public class DeleteBookNotificationsEventHandler : INotificationHandler<DeleteBookEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUserServices _userServices;

        public DeleteBookNotificationsEventHandler(INotificationRepository notificationRepository, IEmailSender emailSender, IUserServices userServices)
        {
            _notificationRepository = notificationRepository;
            _emailSender = emailSender;
            _userServices = userServices;
        }
        public async Task Handle(DeleteBookEvent notification, CancellationToken cancellationToken)
        {
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
                        Subject = $"{notification.Title} with {notification.ISBN} is no longer available in the Library",
                        Body = $"{notification.Title} with {notification.ISBN} is no longer available in the LibraryAll reservations and notifications will be deleted."
                    };

                    var notificationState = await _emailSender.SendEmail(email);
                    await _notificationRepository.DeleteAsync(new Notification { BookId = notification.BookId });
                };
            }
            return;
        }
    }
}
