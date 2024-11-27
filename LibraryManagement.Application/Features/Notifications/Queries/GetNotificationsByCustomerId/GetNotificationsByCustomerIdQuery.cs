using LibraryManagement.Application.Features.Notifications.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByCustomerId
{
    public record GetNotificationsByCustomerIdQuery(string CustomerId) : IRequest<List<NotificationDto>>;
}
