using LibraryManagement.Application.Features.Notifications.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByBookId
{
    public record GetNotifcationsByBookIdQuery(int BookId) : IRequest<List<NotificationDto>>;

}
