using LibraryManagement.Application.Features.Notifications.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Commands.CreateNotification
{
    public class CreateNotificationCommand : IRequest<NotificationDto>
    {
        public int BookId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
    }
}
